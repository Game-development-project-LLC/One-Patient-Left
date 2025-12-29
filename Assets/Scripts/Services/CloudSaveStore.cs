using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

namespace Services
{
    public class CloudSaveStore : MonoBehaviour
    {
        public static CloudSaveStore Instance { get; private set; }

        private const string KEY_STAGE = "stage";
        private const string KEY_INVENTORY_JSON = "inventory_json";

        [Serializable]
        public class ItemStack
        {
            public string itemId;
            public int amount;
        }

        [Serializable]
        private class InventoryPayload
        {
            public List<ItemStack> items = new List<ItemStack>();
        }

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

        public async Task<(int stage, List<ItemStack> inventory)> LoadAsync()
        {
            await UGSBootstrap.Instance.EnsureInitializedAsync();

            var keys = new HashSet<string> { KEY_STAGE, KEY_INVENTORY_JSON };
            var loaded = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

            int stage = 1;
            List<ItemStack> inventory = new List<ItemStack>();

            if (loaded.TryGetValue(KEY_STAGE, out var stageItem) && stageItem != null)
            {
                // CloudSave מחזיר Value בצורה גנרית → ToString ואז parse
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
    }
}
