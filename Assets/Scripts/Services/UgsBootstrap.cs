using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Ensures Unity Gaming Services (UGS) is initialized exactly once and provides
    /// a simple API to await initialization.
    /// </summary>
    /// <remarks>
    /// Initialization is kicked off in Awake and stored as a Task to avoid double initialization
    /// across scenes. This component is a DontDestroyOnLoad singleton.
    /// </remarks>
    public class UGSBootstrap : MonoBehaviour
    {
        /// <summary>Singleton instance (DontDestroyOnLoad).</summary>
        public static UGSBootstrap Instance { get; private set; }

        private Task _initTask;

        /// <summary>True if initialization completed successfully.</summary>
        public bool IsInitialized => _initTask != null && _initTask.IsCompletedSuccessfully;

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

            // Start initialization immediately so first auth/save call is fast.
            _initTask = InitializeAsync();
        }

        /// <summary>
        /// Ensures UGS is initialized. Safe to call multiple times.
        /// </summary>
        public Task EnsureInitializedAsync()
        {
            return _initTask ??= InitializeAsync();
        }

        /// <summary>
        /// Initializes UGS if not already initialized.
        /// </summary>
        private static async Task InitializeAsync()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
                return;

            await UnityServices.InitializeAsync();
        }
    }
}
