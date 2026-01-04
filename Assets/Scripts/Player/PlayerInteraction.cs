using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detects nearby Interactable objects (2D trigger),
/// shows a prompt, and triggers Interact() on key press.
/// Also hides info text when the player moves.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class PlayerInteraction : MonoBehaviour
{
    [Header("Input (New Input System)")]
    [SerializeField] private InputActionReference interactAction;
    [SerializeField] private string interactKeyDisplay = "E";

    [Header("Hide Info On Move")]
    [SerializeField] private float moveHideThreshold = 0.001f;

    private Interactable current;
    private Vector3 lastPosition;
    private bool infoVisible;

    // When the player interacts, we suppress the prompt until they leave the trigger
    // (or until RefreshPrompt() is called, e.g., after closing a notice window).
    private bool suppressPromptUntilExit;

    private void Awake()
    {
        lastPosition = transform.position;
    }

    private void OnEnable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.Enable();
            interactAction.action.performed += OnInteractPerformed;
        }

        // If we were re-enabled (e.g., after a modal lock), try to restore prompt state.
        RefreshPrompt();
    }

    private void OnDisable()
    {
        if (interactAction != null && interactAction.action != null)
        {
            interactAction.action.performed -= OnInteractPerformed;
            interactAction.action.Disable();
        }
    }

    private void Update()
    {
        Vector3 now = transform.position;
        float sqrDelta = (now - lastPosition).sqrMagnitude;

        if (infoVisible && sqrDelta > moveHideThreshold * moveHideThreshold)
        {
            UIManager.Instance?.HideInfo();
            infoVisible = false;
        }

        lastPosition = now;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<Interactable>();
        if (interactable == null) return;

        current = interactable;

        // Don't show prompt while a modal/notice is open, or if we've suppressed it after an interact.
        if (UIManager.Instance != null && UIManager.Instance.IsNoticeOpen) return;
        if (suppressPromptUntilExit) return;

        UIManager.Instance?.ShowPrompt(current.GetPromptText(interactKeyDisplay));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<Interactable>();
        if (interactable == null) return;

        if (current == interactable)
        {
            current = null;
            suppressPromptUntilExit = false;

            UIManager.Instance?.HidePrompt();
            UIManager.Instance?.HideInfo();
            infoVisible = false;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext _)
    {
        // If a notice window is open, ignore interaction.
        if (UIManager.Instance != null && UIManager.Instance.IsNoticeOpen) return;

        if (current == null) return;

        // Hide the "Press E" prompt when interacting so only the new message remains.
        UIManager.Instance?.HidePrompt();

        // Suppress the prompt until we exit the trigger (or RefreshPrompt is called).
        suppressPromptUntilExit = true;

        current.Interact(this);
    }

    // ===== Helpers for interactables =====
    public void ShowInfo(string text)
    {
        UIManager.Instance?.ShowInfo(text);
        infoVisible = !string.IsNullOrWhiteSpace(text);
    }

    public void HideInfo()
    {
        UIManager.Instance?.HideInfo();
        infoVisible = false;
    }

    /// <summary>
    /// Re-shows the prompt for the current interactable (if any), unless a notice is open.
    /// Calling this also clears prompt suppression.
    /// </summary>
    public void RefreshPrompt()
    {
        if (current == null) return;

        // If a notice is open, don't show prompt yet.
        if (UIManager.Instance != null && UIManager.Instance.IsNoticeOpen) return;

        suppressPromptUntilExit = false;
        UIManager.Instance?.ShowPrompt(current.GetPromptText(interactKeyDisplay));
    }

    public string InteractKeyDisplay => interactKeyDisplay;
}
