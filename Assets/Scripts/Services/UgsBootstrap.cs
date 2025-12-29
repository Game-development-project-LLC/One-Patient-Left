using System.Threading.Tasks;
using Unity.Services.Core;
using UnityEngine;

namespace Services
{
    public class UGSBootstrap : MonoBehaviour
    {
        public static UGSBootstrap Instance { get; private set; }

        private Task _initTask;
        public bool IsInitialized => _initTask != null && _initTask.IsCompletedSuccessfully;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
            _initTask = InitializeAsync();
        }

        public Task EnsureInitializedAsync()
        {
            return _initTask ??= InitializeAsync();
        }

        private static async Task InitializeAsync()
        {
            if (UnityServices.State == ServicesInitializationState.Initialized)
                return;

            await UnityServices.InitializeAsync();
        }
    }
}
