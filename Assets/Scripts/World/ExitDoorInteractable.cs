using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Exit door of the ward. Requires a specific inventory item to open
/// (for example a staff keycard).
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

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public override void Interact(PlayerInteraction2D player)
    {
        var inventory = player.GetComponent<PlayerInventory>();

        bool hasRequiredItem =
            inventory != null &&
            !string.IsNullOrWhiteSpace(requiredItemId) &&
            inventory.HasItem(requiredItemId);

        if (!hasRequiredItem)
        {
            UIManager.Instance?.ShowInfo(lockedMessage);
            return;
        }

        UIManager.Instance?.ShowInfo(openMessage);

        if (!string.IsNullOrEmpty(nextSceneName))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}
