using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInventory : MonoBehaviour
{
    [Header("Input (New Input System)")]
    [Tooltip("Button action. Example binding: <Keyboard>/i")]
    [SerializeField] private InputActionReference toggleInventoryAction;

    [Header("Items")]
    [Tooltip("Optional but recommended. Enables AddItem(string id) / HasItem(string id) to resolve to real items.")]
    [SerializeField] private ItemDatabase itemDatabase;

    [Serializable]
    private class InventoryEntry
    {
        public ItemDefinition item;
        [Min(1)] public int amount = 1;
    }

    // List for stable ordering + dictionary for fast lookups.
    [SerializeField] private List<InventoryEntry> entries = new List<InventoryEntry>();
    private Dictionary<ItemDefinition, int> counts = new Dictionary<ItemDefinition, int>();

    private bool inventoryVisible;

    private void Awake()
    {
        RebuildCountsFromEntries();
        SaveGameManager.Instance?.BindInventory(this);
    }

    private void OnEnable()
    {
        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.action.Enable();
            toggleInventoryAction.action.performed += OnToggleInventoryPerformed;
        }
    }

    private void OnDisable()
    {
        if (toggleInventoryAction != null)
        {
            toggleInventoryAction.action.performed -= OnToggleInventoryPerformed;
            toggleInventoryAction.action.Disable();
        }
    }

    private void OnToggleInventoryPerformed(InputAction.CallbackContext ctx)
    {
        inventoryVisible = !inventoryVisible;

        if (inventoryVisible)
            UIManager.Instance?.ShowInfo(GetInventoryText());
        else
            UIManager.Instance?.ClearInfo();
    }

    // ----------------- Public API -----------------

    public void AddItem(string itemId, int amount = 1)
    {
        if (amount <= 0) return;

        ItemDefinition item = ResolveItem(itemId);
        if (item == null)
        {
            Debug.LogWarning($"Inventory: unknown item id '{itemId}'. Did you add it to the ItemDatabase?");
            return;
        }

        AddItem(item, amount);
    }

    public void AddItem(ItemDefinition item, int amount = 1)
    {
        if (item == null) return;
        if (amount <= 0) return;

        int addAmount = amount;

        if (!item.Stackable)
        {
            // Non-stackable items behave like a set: only 0/1.
            if (HasItem(item)) return;
            addAmount = 1;
        }

        int current = GetCount(item);
        int next = Mathf.Min(current + addAmount, item.MaxStack);

        SetCount(item, next);
        RefreshInventoryUIIfVisible();

        SaveGameManager.Instance?.MarkDirty();
    }

    public bool HasItem(string itemId)
    {
        ItemDefinition item = ResolveItem(itemId);
        return item != null && HasItem(item);
    }

    public bool HasItem(ItemDefinition item)
    {
        return item != null && GetCount(item) > 0;
    }

    public int GetCount(string itemId)
    {
        ItemDefinition item = ResolveItem(itemId);
        return item == null ? 0 : GetCount(item);
    }

    public int GetCount(ItemDefinition item)
    {
        if (item == null) return 0;
        return counts.TryGetValue(item, out int c) ? c : 0;
    }

    public bool TryRemoveItem(string itemId, int amount = 1)
    {
        ItemDefinition item = ResolveItem(itemId);
        return TryRemoveItem(item, amount);
    }

    public bool TryRemoveItem(ItemDefinition item, int amount = 1)
    {
        if (item == null) return false;
        if (amount <= 0) return false;

        int current = GetCount(item);
        if (current <= 0) return false;

        int next = Mathf.Max(0, current - amount);
        SetCount(item, next);

        RefreshInventoryUIIfVisible();
        SaveGameManager.Instance?.MarkDirty();
        return true;
    }

    public string GetInventoryText()
    {
        if (counts == null || counts.Count == 0)
            return "Inventory: (empty)";

        StringBuilder sb = new StringBuilder();
        sb.Append("Inventory: ");

        bool first = true;
        foreach (var kvp in counts)
        {
            ItemDefinition item = kvp.Key;
            int amount = kvp.Value;
            if (item == null || amount <= 0) continue;

            if (!first) sb.Append(", ");
            first = false;

            sb.Append(item.DisplayName);
            if (item.Stackable)
                sb.Append($" x{amount}");
        }

        if (first)
            return "Inventory: (empty)";

        return sb.ToString();
    }

    // ----------------- Save/Load helpers -----------------

    public List<SaveGameManager.ItemStack> ExportForSave()
    {
        var list = new List<SaveGameManager.ItemStack>();

        foreach (var kvp in counts)
        {
            ItemDefinition item = kvp.Key;
            int amount = kvp.Value;
            if (item == null || amount <= 0) continue;

            // חייב להיות ID יציב. אם ל-ItemDefinition שלך יש שדה Id / ItemId - תשתמש בו.
            // אם אין, אפשר להשתמש ב-name בתור fallback (אבל עדיף שדה Id).
            string id = !string.IsNullOrWhiteSpace(item.Id) ? item.Id : item.name;

            list.Add(new SaveGameManager.ItemStack { itemId = id, amount = amount });
        }

        return list;
    }

    public void ApplySave(List<SaveGameManager.ItemStack> saved)
    {
        // ננקה את המצב הנוכחי
        entries.Clear();
        counts.Clear();

        if (saved != null)
        {
            foreach (var s in saved)
            {
                if (s == null) continue;
                if (string.IsNullOrWhiteSpace(s.itemId)) continue;
                if (s.amount <= 0) continue;

                AddItem(s.itemId, s.amount);
            }
        }

        RefreshInventoryUIIfVisible();
    }

    // ----------------- Internals -----------------

    private ItemDefinition ResolveItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId)) return null;
        if (itemDatabase == null) return null;
        return itemDatabase.GetById(itemId);
    }

    private void SetCount(ItemDefinition item, int newCount)
    {
        if (item == null) return;

        if (counts == null)
            counts = new Dictionary<ItemDefinition, int>();

        if (newCount <= 0)
        {
            counts.Remove(item);
            RemoveEntry(item);
            return;
        }

        counts[item] = newCount;
        UpsertEntry(item, newCount);
    }

    private void RebuildCountsFromEntries()
    {
        counts = new Dictionary<ItemDefinition, int>();

        foreach (InventoryEntry e in entries)
        {
            if (e == null || e.item == null) continue;
            if (e.amount <= 0) continue;

            int capped = Mathf.Min(e.amount, e.item.MaxStack);
            counts[e.item] = capped;
            e.amount = capped;
        }
    }

    private void UpsertEntry(ItemDefinition item, int amount)
    {
        for (int i = 0; i < entries.Count; i++)
        {
            if (entries[i] != null && entries[i].item == item)
            {
                entries[i].amount = amount;
                return;
            }
        }

        entries.Add(new InventoryEntry { item = item, amount = amount });
    }

    private void RemoveEntry(ItemDefinition item)
    {
        for (int i = entries.Count - 1; i >= 0; i--)
        {
            if (entries[i] != null && entries[i].item == item)
                entries.RemoveAt(i);
        }
    }

    private void RefreshInventoryUIIfVisible()
    {
        if (!inventoryVisible) return;
        UIManager.Instance?.ShowInfo(GetInventoryText());
    }
}
