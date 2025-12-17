using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Exit door of the ward. Can be unlocked by a system (e.g., a switch puzzle),
/// and may optionally require an inventory item (e.g., a staff keycard).
/// If requiredItemId is empty, no item is required.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ExitDoorInteractable : Interactable2D
{
    [Header("Messages")]
    [TextArea]
    [SerializeField]
    private string lockedMessage =
        "The door is locked. You need a staff keycard.";

    [TextArea]
    [SerializeField]
    private string openMessage =
        "You swipe the keycard. The door unlocks.";

    [Header("Requirements")]
    [SerializeField] private string requiredItemId = "staff_keycard";

    [Header("Scene Transition")]
    [SerializeField] private string nextSceneName = string.Empty;

    [SerializeField] private bool isUnlocked = false;

    // Called by puzzles/systems to unlock the door.
    public void SetUnlocked(bool unlocked)
    {
        isUnlocked = unlocked;
    }

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public override void Interact(PlayerInteraction2D player)
    {
        // First gate: must be unlocked by switch/puzzle/system.
        if (!isUnlocked)
        {
            // English message: door is still locked by power/system.
            UIManager.Instance?.ShowInfo("The exit is locked.");
            return;
        }

        // Second gate (optional): require an item ONLY if requiredItemId is set.
        bool requiresItem = !string.IsNullOrWhiteSpace(requiredItemId);
        if (requiresItem)
        {
            var inventory = player.GetComponent<PlayerInventory>();
            bool hasRequiredItem = inventory != null && inventory.HasItem(requiredItemId);

            if (!hasRequiredItem)
            {
                UIManager.Instance?.ShowInfo(lockedMessage);
                return;
            }
        }

        UIManager.Instance?.ShowInfo(openMessage);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
