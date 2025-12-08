using UnityEngine;

/// <summary>
/// Detects nearby Interactable2D objects and triggers Interact() when the key is pressed.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction2D : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private KeyCode interactKey = KeyCode.E;

    private Interactable2D currentTarget;

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponent<Interactable2D>();
        if (interactable == null)
        {
            return;
        }

        currentTarget = interactable;

        // Show the prompt text on screen
        UIManager.Instance?.ShowPrompt(interactable.PromptText);
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
        if (currentTarget == null)
        {
            return;
        }

        if (Input.GetKeyDown(interactKey))
        {
            currentTarget.Interact(this);
        }
    }
}
