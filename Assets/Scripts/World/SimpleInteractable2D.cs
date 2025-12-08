using System;
using UnityEngine;

/// <summary>
/// Generic interactable for simple room objects:
/// - Show text (e.g. window, bed)
/// - Pick up an item (e.g. keycard, photo)
/// - Remove / hide an object (e.g. fake wall)
/// </summary>
public class SimpleInteractable2D : Interactable2D
{
    public enum SimpleInteractionType
    {
        ShowText,
        PickupItem,
        RemoveObject
    }

    [Header("Interaction Type")]
    [SerializeField]
    private SimpleInteractionType interactionType =
        SimpleInteractionType.ShowText;

    [Header("UI Text")]
    [TextArea]
    [SerializeField] private string infoText = string.Empty;

    [Header("Pickup Settings")]
    [SerializeField] private string itemId = string.Empty; // e.g. "staff_keycard", "photo"

    [Header("General Behaviour")]
    [SerializeField] private bool destroyAfterInteract = false;

    [Tooltip("If false, interaction will only work once.")]
    [SerializeField] private bool canInteractMultipleTimes = false;

    [Header("Remove Object Settings")]
    [SerializeField] private GameObject objectToRemove;
    [Tooltip("If true, will just deactivate the object instead of destroying it.")]
    [SerializeField] private bool deactivateInsteadOfDestroy = false;

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

            case SimpleInteractionType.RemoveObject:
                RemoveObject();
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
        var inventory = player.GetComponent<PlayerInventory>();

        if (inventory != null && !string.IsNullOrWhiteSpace(itemId))
        {
            inventory.AddItem(itemId);
        }

        ShowInfoText();
    }

    private void RemoveObject()
    {
        // If no explicit target is set, act on this game object.
        GameObject target = objectToRemove != null ? objectToRemove : gameObject;

        if (deactivateInsteadOfDestroy)
        {
            target.SetActive(false);
        }
        else
        {
            // because this script inherits from MonoBehaviour,
            // we can call Destroy() directly
            Destroy(target);
        }

        ShowInfoText();
    }
}
