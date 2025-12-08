using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very simple inventory: keeps a list of string item IDs,
/// like "photo", "staff_keycard", etc. Attach this to the Player.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Input")]
    [SerializeField] private KeyCode inventoryToggleKey = KeyCode.I;

    // Simple string-based inventory
    private readonly List<string> items = new List<string>();

    // For toggling inventory display
    private bool inventoryVisible = false;

    /// <summary>
    /// Add an item ID to the inventory. Example: AddItem("photo");
    /// </summary>
    public void AddItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return;
        }

        if (!items.Contains(itemId))
        {
            items.Add(itemId);
            Debug.Log($"Inventory: added {itemId}");
        }
    }

    /// <summary>
    /// Check if the inventory contains a given item ID.
    /// Example: HasItem("staff_keycard")
    /// </summary>
    public bool HasItem(string itemId)
    {
        if (string.IsNullOrWhiteSpace(itemId))
        {
            return false;
        }

        return items.Contains(itemId);
    }

    /// <summary>
    /// Text representation of current inventory contents.
    /// </summary>
    public string GetInventoryText()
    {
        if (items.Count == 0)
        {
            return "Inventory: (empty)";
        }

        return "Inventory: " + string.Join(", ", items);
    }

    private void Update()
    {
        // Toggle inventory display
        if (Input.GetKeyDown(inventoryToggleKey))
        {
            inventoryVisible = !inventoryVisible;

            if (inventoryVisible)
            {
                // Show inventory in the InfoText area
                UIManager.Instance?.ShowInfo(GetInventoryText());
            }
            else
            {
                // Hide inventory text (does not hide interaction prompt)
                UIManager.Instance?.ClearInfo();
            }
        }
    }
}
