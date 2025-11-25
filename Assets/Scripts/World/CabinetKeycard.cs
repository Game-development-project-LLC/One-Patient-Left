using UnityEngine;

/// <summary>
/// Simple cabinet that gives the player a keycard when searched.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CabinetKeycard : Interactable2D
{
    [TextArea]
    public string infoMessage = "You found a staff keycard.";

    private bool taken = false;
    private SpriteRenderer spriteRenderer;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Ensure trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public override void Interact(PlayerInteraction2D player)
    {
        if (taken) return;

        taken = true;

        var inventory = player.GetComponent<PlayerInventory>();
        if (inventory != null)
        {
            inventory.AddItem("staff_keycard");
        }
        Level1GameManager.Instance?.GiveKeycard(); 

        if (spriteRenderer != null)
        {
            spriteRenderer.color = Color.gray;
        }

        UIManager.Instance?.ShowInfo(infoMessage);
        UIManager.Instance?.HidePrompt();
    }
}
