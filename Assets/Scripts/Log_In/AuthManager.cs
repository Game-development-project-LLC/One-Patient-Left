using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;

public class AuthManager : MonoBehaviour
{
    public static AuthManager Instance { get; private set; }

    public bool IsSignedIn => AuthenticationService.Instance.IsSignedIn;
    public string PlayerId => AuthenticationService.Instance.PlayerId;

    public event Action SignedIn;
    public event Action SignedOut;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log($"Auth: Signed in. PlayerId={PlayerId}");
            SignedIn?.Invoke();
        };

        AuthenticationService.Instance.SignedOut += () =>
        {
            Debug.Log("Auth: Signed out");
            SignedOut?.Invoke();
        };

        AuthenticationService.Instance.SignInFailed += (e) =>
        {
            Debug.LogError("Auth: SignInFailed: " + e);
        };
    }

    public async Task<string> Register(string username, string password)
    {
        try
        {
            await UgsBootstrap.Instance.InitializeAsync();
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            return "Register successful!";
        }
        catch (AuthenticationException ex) { return $"Register failed: {ex.Message}"; }
        catch (RequestFailedException ex) { return $"Register failed: {ex.Message}"; }
        catch (Exception ex) { return $"Register failed: {ex.Message}"; }
    }

    public async Task<string> Login(string username, string password)
    {
        try
        {
            await UgsBootstrap.Instance.InitializeAsync();
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            return "Login successful!";
        }
        catch (AuthenticationException ex) { return $"Login failed: {ex.Message}"; }
        catch (RequestFailedException ex) { return $"Login failed: {ex.Message}"; }
        catch (Exception ex) { return $"Login failed: {ex.Message}"; }
    }

    public void Logout()
    {
        if (!IsSignedIn) return;
        AuthenticationService.Instance.SignOut();
    }
}
