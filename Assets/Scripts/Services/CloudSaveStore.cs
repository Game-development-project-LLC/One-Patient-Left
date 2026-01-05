using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Services
{
    /// <summary>
    /// Small persistence layer built on top of UGS Cloud Save.
    /// Stores and loads:
    /// <list type="bullet">
    /// <item><description>Current stage (int)</description></item>
    /// <item><description>Inventory payload (JSON string)</description></item>
    /// </list>
    /// </summary>
    /// <remarks>
    /// Cloud Save values are stored as objects and can come back in a generic representation,
    /// therefore we often convert to string and parse.
    /// This component is a DontDestroyOnLoad singleton for easy access from UI and gameplay.
    /// </remarks>
    public class CloudSaveStore : MonoBehaviour
    {
        /// <summary>Singleton instance (DontDestroyOnLoad).</summary>
        public static CloudSaveStore Instance { get; private set; }

        // Cloud Save keys (player data).
        private const string KEY_STAGE = "stage";
        private const string KEY_INVENTORY_JSON = "inventory_json";

        /// <summary>
        /// Minimal representation of an inventory entry.
        /// </summary>
        [Serializable]
        public class ItemStack
        {
            /// <summary>Stable item identifier (e.g., "keycard", "ammo").</summary>
            public string itemId;

            /// <summary>Item quantity.</summary>
            public int amount;
        }

        /// <summary>
        /// JSON payload that Cloud Save stores as a string, because Cloud Save doesn't store
        /// nested collections directly in a typed manner.
        /// </summary>
        [Serializable]
        private class InventoryPayload
        {
            public List<ItemStack> items = new List<ItemStack>();
        }

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
        /// Saves stage + inventory to Cloud Save.
        /// </summary>
        /// <param name="stage">Current stage number (1-based).</param>
        /// <param name="inventory">Inventory list to save. If null, saves empty inventory.</param>
        public async Task SaveAsync(int stage, List<ItemStack> inventory)
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            var payload = new InventoryPayload();
            if (inventory != null) payload.items = inventory;

            string json = JsonUtility.ToJson(payload);

            var data = new Dictionary<string, object>
            {
                { KEY_STAGE, stage },
                { KEY_INVENTORY_JSON, json }
            };

            await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        }

        /// <summary>
        /// Loads stage + inventory from Cloud Save.
        /// If a key does not exist, returns defaults (stage=1, empty inventory).
        /// </summary>
        public async Task<(int stage, List<ItemStack> inventory)> LoadAsync()
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            var keys = new HashSet<string> { KEY_STAGE, KEY_INVENTORY_JSON };
            var loaded = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            int stage = 0;
            List<ItemStack> inventory = new List<ItemStack>();

            if (loaded.TryGetValue(KEY_STAGE, out var stageItem) && stageItem != null)
            {
                // Cloud Save returns a generic Value -> convert to string and parse.
                if (int.TryParse(stageItem.Value?.ToString(), out int s))
                    stage = s;
            }

            if (loaded.TryGetValue(KEY_INVENTORY_JSON, out var invItem) && invItem != null)
            {
                string json = invItem.Value?.ToString();
                if (!string.IsNullOrEmpty(json))
                {
                    var payload = JsonUtility.FromJson<InventoryPayload>(json);
                    if (payload != null && payload.items != null)
                        inventory = payload.items;
                }
            }

            return (stage, inventory);
        }

        /// <summary>
        /// Returns true if we have a non-empty saved stage.
        /// (We treat empty / missing stage as "no save".)
        /// </summary>
        public async Task<bool> HasSaveAsync()
        {
            var (stage, _) = await LoadAsync();
            // A valid save is any stage > 0.
            return stage > 0;
        }

        /// <summary>
        /// Starts a brand-new save (clears inventory) at the given stage/scene name.
        /// </summary>
        public Task SaveNewGameAsync(string startStage)
        {
            // Empty inventory on new game.
            return SaveAsync(1, new List<ItemStack>());
        }

        /// <summary>
        /// Clears the save. We avoid calling CloudSave delete APIs to keep compatibility;
        /// we simply overwrite with an empty stage + empty inventory.
        /// </summary>
        public Task DeleteSaveAsync()
        {
            return SaveAsync(0, new List<ItemStack>());
        }

    }
}
