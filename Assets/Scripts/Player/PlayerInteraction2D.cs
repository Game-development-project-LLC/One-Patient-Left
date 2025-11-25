using UnityEngine;

/// <summary>
/// Handles detecting nearby Interactable2D objects and calling Interact() when E is pressed.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction2D : MonoBehaviour
{
    private Interactable2D currentTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<Interactable2D>();
        if (interactable != null)
        {
            currentTarget = interactable;
            UIManager.Instance?.ShowPrompt(interactable.promptText);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponent<Interactable2D>();
        if (interactable != null && interactable == currentTarget)
        {
            currentTarget = null;
            UIManager.Instance?.HidePrompt();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) && currentTarget != null)
        {
            currentTarget.Interact(this);
        }
    }
}
