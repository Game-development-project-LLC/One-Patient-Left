using UnityEngine;

/// <summary>
/// Base class for any object the player can interact with using the interaction key.
/// </summary>
public abstract class Interactable2D : MonoBehaviour
{
    [Header("Prompt")]
    [TextArea]
    [SerializeField]
    private string promptText = "Press E to interact";

    /// <summary>
    /// Text shown near the player when they can interact with this object.
    /// </summary>
    public string PromptText => promptText;

    /// <summary>
    /// Called by PlayerInteraction2D when the player presses the interaction key.
    /// </summary>
    public abstract void Interact(PlayerInteraction2D player);
}
