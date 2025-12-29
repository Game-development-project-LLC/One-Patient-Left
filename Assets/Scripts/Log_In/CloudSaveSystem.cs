using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

[Serializable]
public class SaveData
{
    public string currentScene;
    public int checkpointId;
    public int health;
    public int ammo;
    public List<string> inventory = new();
}

public static class CloudSaveSystem
{
    private const string KEY_FULL_SAVE = "full_save";
    private const string KEY_CHECKPOINT = "checkpoint_save";

    public static async Task SaveFull(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        await CloudSaveService.Instance.Data.Player.SaveAsync(
            new Dictionary<string, object> { { KEY_FULL_SAVE, json } }
        );
        Debug.Log("Saved FULL to Cloud Save");
    }

    public static async Task<SaveData> LoadFull()
    {
        var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { KEY_FULL_SAVE }
        );

        if (!result.TryGetValue(KEY_FULL_SAVE, out var item))
            return null;

        return JsonUtility.FromJson<SaveData>(item.Value.GetAsString());
    }

    public static async Task SaveCheckpoint(SaveData data)
    {
        string json = JsonUtility.ToJson(data);
        await CloudSaveService.Instance.Data.Player.SaveAsync(
            new Dictionary<string, object> { { KEY_CHECKPOINT, json } }
        );
        Debug.Log("Saved CHECKPOINT to Cloud Save");
    }

    public static async Task<SaveData> LoadCheckpoint()
    {
        var result = await CloudSaveService.Instance.Data.Player.LoadAsync(
            new HashSet<string> { KEY_CHECKPOINT }
        );

        if (!result.TryGetValue(KEY_CHECKPOINT, out var item))
            return null;

        return JsonUtility.FromJson<SaveData>(item.Value.GetAsString());
    }
}
