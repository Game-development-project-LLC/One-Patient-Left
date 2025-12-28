using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Exposes whether the player is currently in stealth (slow-walk) mode.
/// Uses an InputActionReference (e.g., Player/SlowWalk).
/// </summary>
public sealed class PlayerStealthState : MonoBehaviour
{
    [SerializeField] private InputActionReference slowWalkAction;

    public bool IsStealth { get; private set; }

    private void OnEnable()
    {
        var action = slowWalkAction != null ? slowWalkAction.action : null;
        if (action == null) return;

        action.Enable();
        action.performed += OnPerformed;
        action.canceled += OnCanceled;

        // Sync initial state
        IsStealth = action.IsPressed();
    }

    private void OnDisable()
    {
        var action = slowWalkAction != null ? slowWalkAction.action : null;
        if (action == null) return;

        action.performed -= OnPerformed;
        action.canceled -= OnCanceled;
        action.Disable();
    }

    private void OnPerformed(InputAction.CallbackContext ctx) => IsStealth = true;
    private void OnCanceled(InputAction.CallbackContext ctx) => IsStealth = false;
}
