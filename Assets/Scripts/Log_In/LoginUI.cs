using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class LoginUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject signInPanel;
    [SerializeField] private GameObject gamePanel;

    [Header("Fields")]
    [SerializeField] private TMP_InputField usernameField;
    [SerializeField] private TMP_InputField passwordField;
    [SerializeField] private TMP_Text statusText;

    [Header("Optional Buttons")]
    [SerializeField] private GameObject saveButtonGO;
    [SerializeField] private GameObject loadButtonGO;

    private void Start()
    {
        ShowSignIn();
        SetStatus("Enter username + password");
    }

    public async void OnRegisterClicked()
    {
        await HandleAuth(isRegister: true);
    }

    public async void OnLoginClicked()
    {
        await HandleAuth(isRegister: false);
    }

    public void OnLogoutClicked()
    {
        AuthManager.Instance.Logout();
        ShowSignIn();
        SetStatus("Signed out");
    }

    public async void OnSaveClicked()
    {
        await SaveGameManager.Instance.SaveNowAsync();
        SetStatus("Saved!");
    }

    public async void OnLoadClicked()
    {
        bool ok = await SaveGameManager.Instance.LoadAndApplyAsync();
        SetStatus(ok ? "Loaded (if existed)" : "No save found");
    }

    private async Task HandleAuth(bool isRegister)
    {
        string user = usernameField.text?.Trim();
        string pass = passwordField.text ?? "";

        if (string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass))
        {
            SetStatus("Please enter username and password");
            return;
        }

        SetStatus(isRegister ? "Registering..." : "Logging in...");

        string msg = isRegister
            ? await AuthManager.Instance.Register(user, pass)
            : await AuthManager.Instance.Login(user, pass);

        SetStatus(msg);

        if (AuthManager.Instance.IsSignedIn)
        {
            ShowGame();
            SetStatus($"Signed in! PlayerId: {AuthManager.Instance.PlayerId}");

            // טען שמירה אם קיימת
            await SaveGameManager.Instance.LoadAndApplyAsync();
        }
    }

    private void ShowSignIn()
    {
        if (signInPanel != null) signInPanel.SetActive(true);
        if (gamePanel != null) gamePanel.SetActive(false);
    }

    private void ShowGame()
    {
        if (signInPanel != null) signInPanel.SetActive(false);
        if (gamePanel != null) gamePanel.SetActive(true);

        if (saveButtonGO != null) saveButtonGO.SetActive(true);
        if (loadButtonGO != null) loadButtonGO.SetActive(true);
    }

    private void SetStatus(string msg)
    {
        if (statusText != null) statusText.text = msg;
        Debug.Log("LoginUI: " + msg);
    }
}
