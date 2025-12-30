using System;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Central wrapper around Unity Gaming Services (UGS) Authentication.
    /// <para/>
    /// Responsibilities:
    /// <list type="bullet">
    /// <item><description>Ensures UGS is initialized before any auth call.</description></item>
    /// <item><description>Supports username/password registration and login.</description></item>
    /// <item><description>Exposes a simple signed-in state and the current PlayerId.</description></item>
    /// <item><description>Raises events when the user signs in or signs out.</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// This component is implemented as a DontDestroyOnLoad singleton because it is used
    /// by UI screens and gameplay scenes.
    /// </remarks>
    public class AuthManager : MonoBehaviour
    {
        /// <summary>Singleton instance (DontDestroyOnLoad).</summary>
        public static AuthManager Instance { get; private set; }

        /// <summary>True if AuthenticationService exists and the user is currently signed in.</summary>
        public bool IsSignedIn => AuthenticationService.Instance != null && AuthenticationService.Instance.IsSignedIn;

        /// <summary>
        /// The UGS PlayerId of the currently signed-in user.
        /// Returns an empty string when not signed in.
        /// </summary>
        public string PlayerId => IsSignedIn ? AuthenticationService.Instance.PlayerId : string.Empty;

        /// <summary>Raised after a successful sign-in or registration.</summary>
        public event Action SignedIn;

        /// <summary>Raised after signing out.</summary>
        public event Action SignedOut;

        private void Awake()
        {
            // Singleton pattern: keep only one instance across scenes.
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        /// <summary>
        /// Registers a new user using username/password.
        /// </summary>
        /// <param name="username">The desired username.</param>
        /// <param name="password">The desired password.</param>
        public async Task RegisterAsync(string username, string password)
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            // Create a new account and sign in.
            await AuthenticationService.Instance.SignUpWithUsernamePasswordAsync(username, password);

            SignedIn?.Invoke();
        }

        /// <summary>
        /// Signs in an existing user using username/password.
        /// </summary>
        /// <param name="username">The account username.</param>
        /// <param name="password">The account password.</param>
        public async Task LoginAsync(string username, string password)
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            // Sign in to an existing account.
            await AuthenticationService.Instance.SignInWithUsernamePasswordAsync(username, password);

            SignedIn?.Invoke();
        }

        /// <summary>
        /// Signs out the current user.
        /// </summary>
        public void Logout()
        {
            if (!IsSignedIn) return;

            AuthenticationService.Instance.SignOut();
            SignedOut?.Invoke();
        }
    }
}
