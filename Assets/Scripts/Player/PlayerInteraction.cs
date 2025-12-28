using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Detects nearby Interactable objects (2D trigger),
/// shows prompt, and triggers Interact() on key press.
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
        UIManager.Instance?.ShowPrompt(current.GetPromptText(interactKeyDisplay));
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        var interactable = other.GetComponentInParent<Interactable>();
        if (interactable == null) return;

        if (current == interactable)
        {
            current = null;
            UIManager.Instance?.HidePrompt();
            UIManager.Instance?.HideInfo();
            infoVisible = false;
        }
    }

    private void OnInteractPerformed(InputAction.CallbackContext _)
    {
        if (current == null) return;
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

    public void RefreshPrompt()
    {
        if (current == null) return;
        UIManager.Instance?.ShowPrompt(current.GetPromptText(interactKeyDisplay));
    }

    public string InteractKeyDisplay => interactKeyDisplay;
}
