using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace Services
{
    public class AuthManager : MonoBehaviour
    {
        public static AuthManager Instance { get; private set; }

        public bool IsSignedIn => AuthenticationService.Instance != null && AuthenticationService.Instance.IsSignedIn;
        public string PlayerId => IsSignedIn ? AuthenticationService.Instance.PlayerId : string.Empty;

        public event Action SignedIn;
        public event Action SignedOut;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        public async Task RegisterAsync(string username, string password)
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            // יוצר משתמש חדש
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);
            SignedIn?.Invoke();
        }

        public async Task LoginAsync(string username, string password)
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            // התחברות למשתמש קיים
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);
            SignedIn?.Invoke();
        }

        public void Logout()
        {
            if (!IsSignedIn) return;

            AuthenticationService.Instance.SignOut();
            SignedOut?.Invoke();
        }
    }
}
