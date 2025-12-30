using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
    /// <summary>
    /// Minimal UI controller for login/registration and quick Cloud Save actions.
    /// Intended for a simple "Open / Main Menu" scene used during development.
    /// </summary>
    /// <remarks>
    /// The button callbacks are implemented as <c>async void</c> because Unity UI event handlers
    /// do not support <c>Task</c> signatures directly. Exceptions are handled with try/catch and
    /// surfaced to a status text + Debug.Log.
    /// </remarks>
    public class LoginUIController : MonoBehaviour
    {
        [Header("UI")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private Button logoutButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private TMP_Text statusText;

        private void Awake()
        {
            // Wire UI buttons to handlers.
            registerButton.onClick.AddListener(OnRegisterClicked);
            loginButton.onClick.AddListener(OnLoginClicked);
            logoutButton.onClick.AddListener(OnLogoutClicked);
            loadButton.onClick.AddListener(OnLoadClicked);
            saveButton.onClick.AddListener(OnSaveClicked);

            SetStatus("Ready.");
        }

        /// <summary>
        /// Registers a new account and signs in.
        /// </summary>
        private async void OnRegisterClicked()
        {
            try
            {
                SetStatus("Registering...");
                await AuthManager.Instance.RegisterAsync(usernameInput.text, passwordInput.text);
                SetStatus($"Registered & Signed In. PlayerId={AuthManager.Instance.PlayerId}");
            }
            catch (System.Exception e)
            {
                SetStatus("Register failed: " + e.Message);
            }
        }

        /// <summary>
        /// Signs in and immediately loads stage 1 (or you can change this to load from Cloud Save).
        /// </summary>
        private async void OnLoginClicked()
        {
            try
            {
                SetStatus("Logging in...");
                await AuthManager.Instance.LoginAsync(usernameInput.text, passwordInput.text);
                SetStatus($"Signed In. PlayerId={AuthManager.Instance.PlayerId}");

                // After login: start gameplay. You can swap this to LoadAndApplyAsync() if desired.
                await GameFlowManager.Instance.StartGameFromStageAsync(1);
            }
            catch (System.Exception e)
            {
                SetStatus("Login failed: " + e.Message);
            }
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        private void OnLogoutClicked()
        {
            AuthManager.Instance.Logout();
            SetStatus("Signed out.");
        }

        /// <summary>
        /// Loads stage + inventory from Cloud Save and switches to the stage scene.
        /// </summary>
        private async void OnLoadClicked()
        {
            try
            {
                SetStatus("Loading from Cloud Save...");
                await GameFlowManager.Instance.LoadAndApplyAsync();
                SetStatus("Loaded & applied.");
            }
            catch (System.Exception e)
            {
                SetStatus("Load failed: " + e.Message);
            }
        }

        /// <summary>
        /// Saves to Cloud Save.
        /// </summary>
        /// <remarks>
        /// If you click Save in a non-gameplay scene, there may be no PlayerInventory present.
        /// This example saves stage only (inventory = null). For a full save, call
        /// <see cref="GameFlowManager.SaveAsync(PlayerInventory)"/> from inside a gameplay scene.
        /// </remarks>
        private async void OnSaveClicked()
        {
            try
            {
                SetStatus("Saving...");
                await CloudSaveStore.Instance.SaveAsync(GameFlowManager.Instance.CurrentStage, null);
                SetStatus("Saved (stage only).");
            }
            catch (System.Exception e)
            {
                SetStatus("Save failed: " + e.Message);
            }
        }

        /// <summary>
        /// Updates the UI status label and logs to the console.
        /// </summary>
        private void SetStatus(string msg)
        {
            if (statusText != null) statusText.text = msg;
            Debug.Log(msg);
        }
    }
}
