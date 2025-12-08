using UnityEngine;

/// <summary>
/// Generic interactable for simple room objects:
/// - Show text (e.g. window, bed)
/// - Pick up an item (e.g. keycard, photo)
/// </summary>
public class SimpleInteractable2D : Interactable2D
{
    public enum SimpleInteractionType
    {
        ShowText,
        PickupItem
    }

    [Header("Interaction Type")]
    [SerializeField] private SimpleInteractionType interactionType = SimpleInteractionType.ShowText;

    [Header("UI Text")]
    [TextArea]
    [SerializeField] private string infoText = string.Empty;

    [Header("Pickup Settings")]
    [SerializeField] private string itemId = string.Empty; // e.g. "staff_keycard", "photo"

    [Tooltip("Mark that the player has the staff keycard in the level game manager.")]
    [SerializeField] private bool giveKeycardFlag = false;

    [Tooltip("Mark that the player has the photo in the level game manager.")]
    [SerializeField] private bool givePhotoFlag = false;

    [Header("Behaviour")]
    [SerializeField] private bool destroyAfterInteract = false;

    [Tooltip("If false, interaction will only work once.")]
    [SerializeField] private bool canInteractMultipleTimes = false;

    private bool alreadyInteracted = false;

    public override void Interact(PlayerInteraction2D player)
    {
        if (alreadyInteracted && !canInteractMultipleTimes)
        {
            return;
        }

        HandleInteraction(player);

        if (!canInteractMultipleTimes)
        {
            alreadyInteracted = true;
        }

        if (destroyAfterInteract)
        {
            Destroy(gameObject);
        }
    }

    private void HandleInteraction(PlayerInteraction2D player)
    {
        switch (interactionType)
        {
            case SimpleInteractionType.ShowText:
                ShowInfoText();
                break;

            case SimpleInteractionType.PickupItem:
                PickupItem(player);
                break;
        }
    }

    private void ShowInfoText()
    {
        if (!string.IsNullOrWhiteSpace(infoText))
        {
            UIManager.Instance?.ShowInfo(infoText);
        }
    }

    private void PickupItem(PlayerInteraction2D player)
    {
        // נסה למצוא את קומפוננטת האינבנטורי על השחקן
        PlayerInventory inventory = player.GetComponent<PlayerInventory>();

        // Add to inventory if we have an id and an inventory component
        if (!string.IsNullOrWhiteSpace(itemId) && inventory != null)
        {
            inventory.AddItem(itemId);
        }

        
        // Show feedback text
        ShowInfoText();
    }
}
