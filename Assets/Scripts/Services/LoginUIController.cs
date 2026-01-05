using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    /// <summary>
    /// UI controller for Register/Login.
    /// 
    /// Desired behavior:
    /// - Register (new user) -> immediately start from Stage 1 (first scene).
    /// - Login (existing user) -> load last saved progress (stage + inventory) from Cloud Save
    ///   and continue from that stage (defaults to Stage 1 if no save exists yet).
    /// </summary>
    /// <remarks>
    /// Button callbacks are <c>async void</c> because Unity UI events don't support Task signatures.
    /// Exceptions are caught and surfaced to the status label.
    /// </remarks>
    public class LoginUIController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;

        [Header("Buttons")]
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;

        [Header("Optional Buttons (can be null / removed from your menu)")]
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button saveButton;

        [Header("Status")]
        [SerializeField] private TMP_Text statusText;

        private bool _busy;

        private void Awake()
        {
            // Wire required buttons.
            if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
            if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);

            // Optional legacy/dev buttons (only if you still keep them in the scene).
            if (logoutButton != null) logoutButton.onClick.AddListener(OnLogoutClicked);
            if (loadButton != null) loadButton.onClick.AddListener(OnLoadClicked);
            if (saveButton != null) saveButton.onClick.AddListener(OnSaveClicked);

            SetStatus("Ready.");
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;

            if (registerButton != null) registerButton.interactable = !busy;
            if (loginButton != null) loginButton.interactable = !busy;

            if (logoutButton != null) logoutButton.interactable = !busy;
            if (loadButton != null) loadButton.interactable = !busy;
            if (saveButton != null) saveButton.interactable = !busy;
        }

        /// <summary>
        /// Register a new account and immediately start Stage 1.
        /// </summary>
        private async void OnRegisterClicked()
        {
            if (_busy) return;
            SetBusy(true);

            try
            {
                SetStatus("Registering...");
                await AuthManager.Instance.RegisterAsync(usernameInput.text, passwordInput.text);
                SetStatus($"Registered & Signed In. PlayerId={AuthManager.Instance.PlayerId}");

                SetStatus("Starting new game...");
                await GameFlowManager.Instance.StartGameFromStageAsync(1);
            }
            catch (System.Exception e)
            {
                SetStatus("Register failed: " + e.Message);
                Debug.LogException(e);
            }
            finally
            {
                SetBusy(false);
            }
        }

        /// <summary>
        /// Login and continue from last saved stage (Cloud Save).
        /// </summary>
        private async void OnLoginClicked()
        {
            if (_busy) return;
            SetBusy(true);

            try
            {
                SetStatus("Logging in...");
                await AuthManager.Instance.LoginAsync(usernameInput.text, passwordInput.text);
                SetStatus($"Signed In. PlayerId={AuthManager.Instance.PlayerId}");

                // Continue from last save (defaults to stage 1 if no save exists).
                SetStatus("Loading last checkpoint...");
                await GameFlowManager.Instance.LoadAndApplyAsync();
            }
            catch (System.Exception e)
            {
                SetStatus("Login failed: " + e.Message);
                Debug.LogException(e);
            }
            finally
            {
                SetBusy(false);
            }
        }

        // -----------------------------
        // Optional / dev buttons
        // -----------------------------

        private void OnLogoutClicked()
        {
            if (_busy) return;
            AuthManager.Instance.Logout();
            SetStatus("Signed out.");
        }

        private async void OnLoadClicked()
        {
            if (_busy) return;
            SetBusy(true);

            try
            {
                SetStatus("Loading from Cloud Save...");
                await GameFlowManager.Instance.LoadAndApplyAsync();
                SetStatus("Loaded & applied.");
            }
            catch (System.Exception e)
            {
                SetStatus("Load failed: " + e.Message);
                Debug.LogException(e);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async void OnSaveClicked()
        {
            if (_busy) return;
            SetBusy(true);

            try
            {
                SetStatus("Saving...");
                await CloudSaveStore.Instance.SaveAsync(GameFlowManager.Instance.CurrentStage, null);
                SetStatus("Saved (stage only).");
            }
            catch (System.Exception e)
            {
                SetStatus("Save failed: " + e.Message);
                Debug.LogException(e);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetStatus(string msg)
        {
            if (statusText != null) statusText.text = msg;
            Debug.Log(msg);
        }
    }
}
