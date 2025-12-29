using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Services
{
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
            registerButton.onClick.AddListener(OnRegisterClicked);
            loginButton.onClick.AddListener(OnLoginClicked);
            logoutButton.onClick.AddListener(OnLogoutClicked);
            loadButton.onClick.AddListener(OnLoadClicked);
            saveButton.onClick.AddListener(OnSaveClicked);

            SetStatus("Ready.");
        }

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

        private async void OnLoginClicked()
        {
            try
            {
                SetStatus("Logging in...");
                await AuthManager.Instance.LoginAsync(usernameInput.text, passwordInput.text);
                SetStatus($"Signed In. PlayerId={AuthManager.Instance.PlayerId}");

                // אחרי login: התחל משחק (stage 1) או לטעון מהענן אם אתה רוצה
                await GameFlowManager.Instance.StartGameFromStageAsync(1);
            }
            catch (System.Exception e)
            {
                SetStatus("Login failed: " + e.Message);
            }
        }

        private void OnLogoutClicked()
        {
            AuthManager.Instance.Logout();
            SetStatus("Signed out.");
        }

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

        private async void OnSaveClicked()
        {
            try
            {
                SetStatus("Saving...");
                // אם אתה לוחץ Save במסך Open, אין PlayerInventory בסצנה הזו.
                // אז Save אמיתי תעשה מתוך Level_1 (או תעשה רק stage כאן).
                await CloudSaveStore.Instance.SaveAsync(GameFlowManager.Instance.CurrentStage, null);
                SetStatus("Saved (stage only).");
            }
            catch (System.Exception e)
            {
                SetStatus("Save failed: " + e.Message);
            }
        }

        private void SetStatus(string msg)
        {
            if (statusText != null) statusText.text = msg;
            Debug.Log(msg);
        }
    }
}
