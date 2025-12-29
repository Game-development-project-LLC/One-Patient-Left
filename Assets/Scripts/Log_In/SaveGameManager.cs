using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SaveGameManager : MonoBehaviour
{
    public static SaveGameManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private PlayerInventory playerInventory;

    [Header("Save Settings")]
    [SerializeField] private string saveKey = "one_last_patient_save_v1";
    [SerializeField] private float debounceSeconds = 0.5f;

    private bool dirty;
    private Coroutine pendingSave;
    private SaveData pendingLoadedData;

    [Serializable]
    public class ItemStack
    {
        public string itemId;
        public int amount;
    }

    [Serializable]
    public class SaveData
    {
        public string sceneName;
        public int sceneBuildIndex;
        public List<ItemStack> inventory = new();
        public long savedUtcTicks;
    }

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void BindInventory(PlayerInventory inv) => playerInventory = inv;

    public void MarkDirty()
    {
        dirty = true;
        if (pendingSave == null)
            pendingSave = StartCoroutine(SaveAfterDelay());
    }

    private IEnumerator SaveAfterDelay()
    {
        yield return new WaitForSeconds(debounceSeconds);
        pendingSave = null;

        if (dirty)
            _ = SaveNowAsync();
    }

    public async Task SaveNowAsync()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsSignedIn)
        {
            Debug.Log("SaveGame: Not signed in - skipping save");
            return;
        }

        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogWarning("SaveGame: PlayerInventory not found - skipping save");
            return;
        }

        SaveData data = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            sceneBuildIndex = SceneManager.GetActiveScene().buildIndex,
            inventory = playerInventory.ExportForSave(),
            savedUtcTicks = DateTime.UtcNow.Ticks
        };

        string json = JsonUtility.ToJson(data);
        await CloudSaveStore.SaveJson(saveKey, json);

        dirty = false;
        Debug.Log($"SaveGame: Saved. scene='{data.sceneName}', items={data.inventory.Count}");
    }

    public async Task<bool> LoadAndApplyAsync()
    {
        if (AuthManager.Instance == null || !AuthManager.Instance.IsSignedIn)
        {
            Debug.LogWarning("SaveGame: Load requested but not signed in");
            return false;
        }

        string json = await CloudSaveStore.LoadJson(saveKey);
        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("SaveGame: No save found");
            return false;
        }

        SaveData data = JsonUtility.FromJson<SaveData>(json);
        if (data == null)
        {
            Debug.LogWarning("SaveGame: Failed to parse save JSON");
            return false;
        }

        // אם צריך לעבור סצנה – נטען סצנה ואז ניישם אינבנטורי אחרי שהסצנה נטענה
        string currentScene = SceneManager.GetActiveScene().name;
        if (!string.Equals(currentScene, data.sceneName, StringComparison.Ordinal))
        {
            pendingLoadedData = data;
            SceneManager.sceneLoaded += OnSceneLoadedApply;
            SceneManager.LoadScene(data.sceneName);
            return true;
        }

        ApplyInventory(data);
        return true;
    }

    private void OnSceneLoadedApply(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedApply;

        // תן פריים אחד כדי שכל האובייקטים ייווצרו
        StartCoroutine(ApplyAfterOneFrame());
    }

    private IEnumerator ApplyAfterOneFrame()
    {
        yield return null;

        if (pendingLoadedData != null)
        {
            ApplyInventory(pendingLoadedData);
            pendingLoadedData = null;
        }
    }

    private void ApplyInventory(SaveData data)
    {
        if (playerInventory == null)
            playerInventory = FindFirstObjectByType<PlayerInventory>();

        if (playerInventory == null)
        {
            Debug.LogWarning("SaveGame: Could not find PlayerInventory to apply save");
            return;
        }

        playerInventory.ApplySave(data.inventory);
        Debug.Log($"SaveGame: Applied inventory ({data.inventory.Count} items)");
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause) _ = SaveNowAsync();
    }

    private void OnApplicationQuit()
    {
        // best-effort (לא תמיד יספיק ב-web)
        _ = SaveNowAsync();
    }
}
