using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Exit door of the ward. Requires a keycard to open.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ExitDoorInteractable : Interactable2D
{
    [TextArea]
    public string lockedMessage = "The door is locked. You need a staff keycard.";
    [TextArea]
    public string openMessage = "You swipe the keycard. The door unlocks.";

    // Optional: name of the next scene to load when exiting
    public string nextSceneName = "";

    private void Awake()
    {
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    public override void Interact(PlayerInteraction2D player)
    {
        var inventory = player.GetComponent<PlayerInventory>();

        bool hasKey =
            (inventory != null && inventory.HasItem("staff_keycard"))
            || (Level1GameManager.Instance != null && Level1GameManager.Instance.HasKeycard);

        if (!hasKey)
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
