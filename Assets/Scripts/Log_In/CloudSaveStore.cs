using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.CloudSave;
using UnityEngine;

public static class CloudSaveStore
{
    public static async Task SaveJson(string key, string json)
    {
        var data = new Dictionary<string, object> { { key, json } };
        await CloudSaveService.Instance.Data.Player.SaveAsync(data);
        Debug.Log($"CloudSave: Saved key='{key}' ({json.Length} chars)");
    }

    public static async Task<string> LoadJson(string key)
    {
        var keys = new HashSet<string> { key };
        var result = await CloudSaveService.Instance.Data.Player.LoadAsync(keys);

        if (!result.TryGetValue(key, out var item))
            return null;

        return item.Value.GetAs<string>();
    }

    public static async Task Delete(string key)
    {
        await CloudSaveService.Instance.Data.Player.DeleteAsync(key);
    }
}
