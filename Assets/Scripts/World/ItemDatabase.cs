using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "OnePatientLeft/Items/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<ItemDefinition> items = new List<ItemDefinition>();

    private Dictionary<string, ItemDefinition> byId;

    private void OnEnable()
    {
        RebuildLookup();
    }

    public void RebuildLookup()
    {
        byId = new Dictionary<string, ItemDefinition>();
        foreach (ItemDefinition item in items)
        {
            if (item == null) continue;
            if (string.IsNullOrWhiteSpace(item.Id)) continue;

            // Last one wins if duplicates exist
            byId[item.Id] = item;
        }
    }

    public ItemDefinition GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;

        if (byId == null || byId.Count == 0)
            RebuildLookup();

        return byId.TryGetValue(id, out ItemDefinition item) ? item : null;
    }

    public IReadOnlyList<ItemDefinition> Items => items;
}
