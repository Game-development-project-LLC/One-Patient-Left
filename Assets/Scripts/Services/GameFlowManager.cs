using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services
{
    public class GameFlowManager : MonoBehaviour
    {
        public static GameFlowManager Instance { get; private set; }

        [Header("Stage -> Scene mapping (index = stage-1)")]
        [SerializeField] private string[] stageSceneNames = { "Level_1" };

        public int CurrentStage { get; private set; } = 1;

        // inventory שמגיע מ-Load ומיושם כשנכנסים לסצנת המשחק
        private List<CloudSaveStore.ItemStack> _pendingInventory;

        private void Awake()
        {
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

        public string GetSceneForStage(int stage)
        {
            int idx = Mathf.Clamp(stage - 1, 0, stageSceneNames.Length - 1);
            return stageSceneNames[idx];
        }

        public async Task StartGameFromStageAsync(int stage)
        {
            CurrentStage = Mathf.Max(1, stage);
            string sceneName = GetSceneForStage(CurrentStage);
            await LoadSceneAsync(sceneName);
        }

        public async Task SaveAsync(PlayerInventory inventory)
        {
            var inv = inventory != null
                ? ConvertInventory(inventory.GetItemsForSave())
                : new List<CloudSaveStore.ItemStack>();

            await CloudSaveStore.Instance.SaveAsync(CurrentStage, inv);
        }

        public async Task LoadAndApplyAsync()
        {
            var (stage, inv) = await CloudSaveStore.Instance.LoadAsync();

            CurrentStage = Mathf.Max(1, stage);
            _pendingInventory = inv;

            string sceneName = GetSceneForStage(CurrentStage);
            await LoadSceneAsync(sceneName);
        }

        private async Task LoadSceneAsync(string sceneName)
        {
            var op = SceneManager.LoadSceneAsync(sceneName);
            while (!op.isDone) await Task.Yield();
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            if (_pendingInventory == null) return;

            // חפש PlayerInventory בסצנה החדשה ויישם
            var inv = FindFirstObjectByType<PlayerInventory>();
            if (inv != null)
            {
                inv.ApplySaveStacks(ConvertBack(_pendingInventory));
                _pendingInventory = null;
            }
        }

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
