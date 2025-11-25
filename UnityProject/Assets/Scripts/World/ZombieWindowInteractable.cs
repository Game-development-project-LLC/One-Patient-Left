using UnityEngine;

/// <summary>
/// Window with a zombie behind glass. Only shows a story / warning message.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZombieWindowInteractable : Interactable2D
{
    [TextArea]
    public string infoMessage = "A zombie slams into the glass. Better stay quiet...";

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public override void Interact(PlayerInteraction2D player)
    {
        UIManager.Instance?.ShowInfo(infoMessage);
    }
}
