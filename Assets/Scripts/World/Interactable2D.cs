using UnityEngine;

/// <summary>
/// Base class for any object the player can interact with using the E key.
/// </summary>
public abstract class Interactable2D : MonoBehaviour
{
    [TextArea]
    public string promptText = "Press E to interact";

    /// <summary>
    /// Called by PlayerInteraction2D when the player presses E near this object.
    /// </summary>
    public abstract void Interact(PlayerInteraction2D player);
}
