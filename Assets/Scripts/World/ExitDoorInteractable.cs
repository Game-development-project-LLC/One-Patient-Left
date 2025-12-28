//using UnityEngine;

//public class ExitDoorInteractable : Interactable
//{
//    [Header("Requirement")]
//    [Tooltip("Preferred: assign an ItemDefinition asset (e.g., StaffKeycard).")]
//    [SerializeField] private ItemDefinition requiredItem;

//    [Tooltip("Fallback for older content. Used only if Required Item is not assigned.")]
//    [SerializeField] private string requiredItemId = "staff_keycard";

//    [Header("State")]
//    [Tooltip("If true, door is already unlocked (e.g., by a puzzle).")]
//    [SerializeField] private bool isUnlocked = false;

//    [Header("Messages")]
//    [SerializeField] private string unlockedMessage = "Door unlocked! You can exit now.";

//    [Tooltip("If empty, a message will be generated automatically using the required item's name.")]
//    [SerializeField] private string lockedMessageOverride = "";

//    /// <summary>
//    /// Allows other systems (e.g., puzzles) to unlock/lock the door.
//    /// SwitchPanelPuzzle expects this method to exist.
//    /// </summary>
//    public void SetUnlocked(bool value)
//    {
//        isUnlocked = value;
//    }

//    public override void Interact(PlayerInteraction player)
//    {
//        if (player == null)
//            return;

//        // If already unlocked (e.g., puzzle solved), allow exit immediately.
//        if (isUnlocked)
//        {
//            UIManager.Instance?.ShowInfo(unlockedMessage);
//            return;
//        }

//        PlayerInventory inventory = player.GetComponent<PlayerInventory>();
//        if (inventory == null)
//        {
//            Debug.LogWarning("ExitDoorInteractable: PlayerInventory not found on player.");
//            return;
//        }

//        bool hasRequirement = HasRequiredItem(inventory);

//        if (hasRequirement)
//        {
//            UIManager.Instance?.ShowInfo(unlockedMessage);

//            // Optional: if you want to consume the keycard, uncomment:
//            // ConsumeRequiredItem(inventory);
//        }
//        else
//        {
//            UIManager.Instance?.ShowInfo(GetLockedMessage());
//        }
//    }

//    private bool HasRequiredItem(PlayerInventory inventory)
//    {
//        if (requiredItem != null)
//            return inventory.HasItem(requiredItem);

//        if (!string.IsNullOrWhiteSpace(requiredItemId))
//            return inventory.HasItem(requiredItemId);

//        // No requirement set
//        return true;
//    }

//    private void ConsumeRequiredItem(PlayerInventory inventory)
//    {
//        if (requiredItem != null)
//            inventory.TryRemoveItem(requiredItem, 1);
//        else if (!string.IsNullOrWhiteSpace(requiredItemId))
//            inventory.TryRemoveItem(requiredItemId, 1);
//    }

//    private string GetLockedMessage()
//    {
//        if (!string.IsNullOrWhiteSpace(lockedMessageOverride))
//            return lockedMessageOverride;

//        if (requiredItem != null)
//            return $"The door is locked. You need: {requiredItem.DisplayName}.";

//        if (!string.IsNullOrWhiteSpace(requiredItemId))
//            return $"The door is locked. You need: {requiredItemId}.";

//        return "The door is locked.";
//    }
//}
