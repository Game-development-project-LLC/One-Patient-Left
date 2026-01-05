using System;
using System.Threading.Tasks;
using Services;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    /// <summary>
    /// Controls the "Open" scene UI flow:
    /// 1) SignInPanel (username + password + Register/Login)
    /// 2) LargeBoard (Intro panel with Start New / Load)
    ///
    /// The two panels live in the same scene, but only one is visible at a time.
    /// </summary>
    public sealed class LoginUIController : MonoBehaviour
    {
        [Header("Panels")]
        [SerializeField] private GameObject signInPanel;
        [SerializeField] private GameObject introPanel;

        [Header("Sign-in UI")]
        [SerializeField] private TMP_InputField usernameInput;
        [SerializeField] private TMP_InputField passwordInput;
        [SerializeField] private Button registerButton;
        [SerializeField] private Button loginButton;
        [SerializeField] private TMP_Text signInStatusText;

        [Header("Intro UI")]
        [SerializeField] private Button startNewButton;
        [SerializeField] private Button loadButton;
        [SerializeField] private TMP_Text introStatusText; // optional (can be null)

        private bool _busy;

        private void Reset()
        {
            // Try to auto-wire common defaults when the script is first added.
            signInPanel = gameObject;
        }

        private void Awake()
        {
            // If you forgot to set panels, fall back to the current GameObject for sign-in.
            if (signInPanel == null) signInPanel = gameObject;

            if (registerButton != null) registerButton.onClick.AddListener(OnRegisterClicked);
            if (loginButton != null) loginButton.onClick.AddListener(OnLoginClicked);
            if (startNewButton != null) startNewButton.onClick.AddListener(OnStartNewClicked);
            if (loadButton != null) loadButton.onClick.AddListener(OnLoadClicked);
        }

        private async void Start()
        {
            ShowSignIn();

            // If user is already signed in (Editor play mode / session restore), skip straight to intro panel.
            try
            {
                if (AuthManager.Instance != null && AuthManager.Instance.IsSignedIn)
                    await ShowIntroPanelAsync();
            }
            catch
            {
                // ignore
            }
        }

        private void ShowSignIn()
        {
            if (signInPanel != null) signInPanel.SetActive(true);
            if (introPanel != null) introPanel.SetActive(false);

            SetSignInStatus("");
            SetIntroStatus("");
            SetButtonsInteractable(true);
        }

        private async Task ShowIntroPanelAsync()
        {
            if (signInPanel != null) signInPanel.SetActive(false);
            if (introPanel != null) introPanel.SetActive(true);

            SetSignInStatus("");
            await RefreshLoadButtonAsync();
        }

        private async Task RefreshLoadButtonAsync()
        {
            bool hasSave = false;

            if (CloudSaveStore.Instance != null)
                hasSave = await CloudSaveStore.Instance.HasSaveAsync();

            if (loadButton != null)
                loadButton.interactable = hasSave && !_busy;

            if (!hasSave)
                SetIntroStatus("No saved progress found.");
            else
                SetIntroStatus("");
        }

        private void SetButtonsInteractable(bool enabled)
        {
            if (registerButton != null) registerButton.interactable = enabled;
            if (loginButton != null) loginButton.interactable = enabled;

            if (startNewButton != null) startNewButton.interactable = enabled;
            if (loadButton != null) loadButton.interactable = enabled;
        }

        private void SetSignInStatus(string msg)
        {
            if (signInStatusText != null) signInStatusText.text = msg ?? "";
        }

        private void SetIntroStatus(string msg)
        {
            if (introStatusText != null) introStatusText.text = msg ?? "";
        }

        private string Username => usernameInput != null ? usernameInput.text.Trim() : "";
        private string Password => passwordInput != null ? passwordInput.text : "";

        private async void OnRegisterClicked()
        {
            if (_busy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                SetSignInStatus("Please enter a username and password.");
                return;
            }

            await RunBusyAsync(async () =>
            {
                SetSignInStatus("Registering...");
                await AuthManager.Instance.RegisterAsync(Username, Password);
                SetSignInStatus("");
                await ShowIntroPanelAsync();
            });
        }

        private async void OnLoginClicked()
        {
            if (_busy) return;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(Password))
            {
                SetSignInStatus("Please enter a username and password.");
                return;
            }

            await RunBusyAsync(async () =>
            {
                SetSignInStatus("Signing in...");
                await AuthManager.Instance.LoginAsync(Username, Password);
                SetSignInStatus("");
                await ShowIntroPanelAsync();
            });
        }

        private async void OnStartNewClicked()
        {
            if (_busy) return;

            await RunBusyAsync(async () =>
            {
                SetIntroStatus("Starting new run...");

                if (CloudSaveStore.Instance != null)
                {
                    // Reset previous progress (if any), then initialize a clean save at stage 1.
                    await CloudSaveStore.Instance.DeleteSaveAsync();
                    await CloudSaveStore.Instance.SaveNewGameAsync(startStage: "1");
                }

                // Go to the first stage scene.
                if (GameFlowManager.Instance != null)
                    await GameFlowManager.Instance.StartGameFromStageAsync(1);
            });
        }

        private async void OnLoadClicked()
        {
            if (_busy) return;

            await RunBusyAsync(async () =>
            {
                bool hasSave = CloudSaveStore.Instance != null && await CloudSaveStore.Instance.HasSaveAsync();
                if (!hasSave)
                {
                    SetIntroStatus("Nothing to load yet.");
                    return;
                }

                SetIntroStatus("Loading...");
                if (GameFlowManager.Instance != null)
                    await GameFlowManager.Instance.LoadAndApplyAsync();
            });
        }

        private async Task RunBusyAsync(Func<Task> work)
        {
            _busy = true;
            SetButtonsInteractable(false);

            try
            {
                await work();
            }
            catch (Exception e)
            {
                // Keep error messages short for UI.
                SetSignInStatus($"Error: {e.Message}");
                SetIntroStatus($"Error: {e.Message}");
            }
            finally
            {
                _busy = false;

                // Re-enable whatever panel is currently visible.
                if (signInPanel != null && signInPanel.activeInHierarchy)
                {
                    SetButtonsInteractable(true);
                }
                else
                {
                    SetButtonsInteractable(true);
                    await RefreshLoadButtonAsync();
                }
            }
        }
    }
}
