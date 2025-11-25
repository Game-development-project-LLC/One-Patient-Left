using UnityEngine;

/// <summary>
/// A simple "photo" object near the bed.
/// When the player presses E on it for the first time:
/// - The photo is added to the level state (HasPhoto = true)
/// - The sprite disappears (photo taken)
/// - A short story message is shown on screen.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PhotoPickup : Interactable2D
{
    [TextArea]
    public string infoMessage =
        "You found an old photo of you and the other patients. Someone's face is scratched out.";

    private bool taken = false;
    private SpriteRenderer spriteRenderer;
    private Collider2D col;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        col.isTrigger = true; // make sure this is a trigger
    }

    public override void Interact(PlayerInteraction2D player)
    {
        if (taken) return;

        taken = true;

        // Get the inventory from the player
        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem("photo");
        }

        Level1GameManager.Instance?.GivePhoto();

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (col != null)
            col.enabled = false;

        UIManager.Instance?.ShowInfo(infoMessage);
        UIManager.Instance?.HidePrompt();
    }
}
