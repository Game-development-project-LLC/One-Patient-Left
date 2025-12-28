using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Press interact near the object -> show a message (from Inspector).
/// Message disappears when the player moves (handled by PlayerInteraction).
/// </summary>
public class TextOnInteract : Interactable
{
    [Header("Message")]
    [TextArea(2, 6)]
    [SerializeField] private string message = "Some text...";

    [Tooltip("Optional extra behavior when the player interacts.")]
    [SerializeField] private UnityEvent onInteracted;

    public override void Interact(PlayerInteraction interactor)
    {
        if (interactor == null) return;

        interactor.ShowInfo(message);
        onInteracted?.Invoke();
    }
}
