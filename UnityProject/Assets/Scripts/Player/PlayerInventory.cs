using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Very simple inventory: keeps a list of string item IDs,
/// like "photo", "staff_keycard", etc.
/// Attach this to the Player.
/// Press I to show/hide inventory contents on screen.
/// </summary>
public class PlayerInventory : MonoBehaviour
{
    // Simple string-based inventory
    private List<string> items = new List<string>();

    // For toggling inventory display
    private bool inventoryVisible = false;

    /// <summary>
    /// Add an item ID to the inventory.
    /// Example: AddItem("photo");
    /// </summary>
    public void AddItem(string itemId)
    {
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
        return items.Contains(itemId);
    }

    /// <summary>
    /// Optional: remove item if you ever need it.
    /// </summary>
    public bool RemoveItem(string itemId)
    {
        return items.Remove(itemId);
    }

    /// <summary>
    /// Returns inventory contents as a readable string.
    /// </summary>
    public string GetInventoryText()
    {
        if (items.Count == 0)
            return "Inventory: (empty)";

        return "Inventory: " + string.Join(", ", items);
    }

    private void Update()
    {
        // Press I to toggle inventory display
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventoryVisible = !inventoryVisible;

            if (inventoryVisible)
            {
                // Show inventory in the InfoText area
                UIManager.Instance?.ShowInfo(GetInventoryText());
            }
            else
            {
                // Hide inventory text (does not hide prompt)
                UIManager.Instance?.ClearInfo();
            }
        }
    }
}
