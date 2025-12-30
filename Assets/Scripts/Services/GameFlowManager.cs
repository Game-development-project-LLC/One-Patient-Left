using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    /// <summary>
    /// Manages stage progression, scene loading, and applying Cloud Save data into the gameplay scene.
    /// </summary>
    /// <remarks>
    /// Workflow:
    /// <list type="number">
    /// <item><description>Load stage + inventory from Cloud Save.</description></item>
    /// <item><description>Load the correct scene for that stage.</description></item>
    /// <item><description>When the scene finishes loading, find PlayerInventory and apply the saved inventory.</description></item>
    /// </list>
    /// This component is a DontDestroyOnLoad singleton because it coordinates multiple scenes.
    /// </remarks>
    public class GameFlowManager : MonoBehaviour
    {
        /// <summary>Singleton instance (DontDestroyOnLoad).</summary>
        public static GameFlowManager Instance { get; private set; }

        [Header("Stage -> Scene mapping (index = stage-1)")]
        [Tooltip("Scene names for each stage. Index 0 = stage 1, index 1 = stage 2, etc.")]
        [SerializeField] private string[] stageSceneNames = { "Level_1" };

        /// <summary>Current stage number (1-based).</summary>
        public int CurrentStage { get; private set; } = 1;

        /// <summary>
        /// Inventory data loaded from Cloud Save but not yet applied.
        /// It will be applied once the target gameplay scene loads.
        /// </summary>
        private List<CloudSaveStore.ItemStack> _pendingInventory;

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

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        /// <summary>
        /// Returns the scene name for a given stage number.
        /// If stage is outside the configured range, clamps to the nearest valid scene.
        /// </summary>
        public string GetSceneForStage(int stage)
        {
            int idx = Mathf.Clamp(stage - 1, 0, stageSceneNames.Length - 1);
            return stageSceneNames[idx];
        }

        /// <summary>
        /// Starts the game by loading the scene mapped to the given stage.
        /// </summary>
        public async Task StartGameFromStageAsync(int stage)
        {
            CurrentStage = Mathf.Max(1, stage);
            string sceneName = GetSceneForStage(CurrentStage);
            await LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// Saves current stage + inventory to Cloud Save.
        /// </summary>
        /// <param name="inventory">Reference to the player's inventory component (can be null).</param>
        public async Task SaveAsync(PlayerInventory inventory)
        {
            var inv = inventory != null
                ? ConvertInventory(inventory.GetItemsForSave())
                : new List<CloudSaveStore.ItemStack>();

            await CloudSaveStore.Instance.SaveAsync(CurrentStage, inv);
        }

        /// <summary>
        /// Loads stage + inventory from Cloud Save and applies it by loading the stage scene.
        /// </summary>
        public async Task LoadAndApplyAsync()
        {
            var (stage, inv) = await CloudSaveStore.Instance.LoadAsync();

            CurrentStage = Mathf.Max(1, stage);
            _pendingInventory = inv;

            string sceneName = GetSceneForStage(CurrentStage);
            await LoadSceneAsync(sceneName);
        }

        /// <summary>
        /// Loads a Unity scene asynchronously and awaits completion.
        /// </summary>
        private static async Task LoadSceneAsync(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) await Task.Yield();
        }

        /// <summary>
        /// Scene loaded callback used to apply pending inventory when we enter a gameplay scene.
        /// </summary>
        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_pendingInventory == null) return;

            // Find the inventory component in the new scene and apply the saved items.
            var inv = FindFirstObjectByType<PlayerInventory>();
            if (inv != null)
            {
                inv.ApplySaveStacks(ConvertBack(_pendingInventory));
                _pendingInventory = null;
            }
        }

        /// <summary>
        /// Converts the runtime inventory representation into CloudSaveStore.ItemStack for serialization.
        /// </summary>
        private static List<CloudSaveStore.ItemStack> ConvertInventory(List<PlayerInventory.SimpleItemStack> items)
        {
            var list = new List<CloudSaveStore.ItemStack>();
            if (items == null) return list;

            foreach (var s in items)
            {
                if (s == null) continue;
                if (string.IsNullOrWhiteSpace(s.itemId)) continue;
                if (s.amount <= 0) continue;

                list.Add(new CloudSaveStore.ItemStack { itemId = s.itemId, amount = s.amount });
            }

            return list;
        }

        /// <summary>
        /// Converts CloudSaveStore.ItemStack back into the runtime inventory representation.
        /// </summary>
        private static List<PlayerInventory.SimpleItemStack> ConvertBack(List<CloudSaveStore.ItemStack> items)
        {
            var list = new List<PlayerInventory.SimpleItemStack>();
            if (items == null) return list;

            foreach (var s in items)
            {
                if (s == null) continue;
                list.Add(new PlayerInventory.SimpleItemStack { itemId = s.itemId, amount = s.amount });
            }

            return list;
        }
    }
}
