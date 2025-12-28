using UnityEngine;

/// <summary>
/// Base class for any object the player can interact with (press a key).
/// Prompt supports {key} placeholder (e.g., "Press {key} to interact").
/// </summary>
public abstract class Interactable : MonoBehaviour
{
    [Header("Prompt")]
    [SerializeField] private string promptTemplate = "Press {key} to interact";

    /// <summary>
    /// Raw prompt template (may include {key}).
    /// </summary>
    public string PromptTemplate => promptTemplate;

    /// <summary>
    /// Returns the prompt text with the current key injected.
    /// </summary>
    public string GetPromptText(string keyDisplay)
    {
        var key = string.IsNullOrWhiteSpace(keyDisplay) ? "E" : keyDisplay;
        return (promptTemplate ?? string.Empty).Replace("{key}", key);
    }

    /// <summary>
    /// Called when the player presses interact while in range.
    /// </summary>
    public abstract void Interact(PlayerInteraction interactor);
}
