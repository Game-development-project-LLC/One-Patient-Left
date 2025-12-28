using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Touch (trigger enter) -> show message and invoke events.
/// Useful for clues, traps, noise makers, etc.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class TouchActionTrigger2D : MonoBehaviour
{
    [Header("Who can trigger")]
    [SerializeField] private string playerTag = "Player";

    [Header("UI")]
    [TextArea(2, 6)]
    [SerializeField] private string messageOnTouch = "Something happened...";

    [Header("Behavior")]
    [SerializeField] private bool oneShot = true;
    [SerializeField] private bool disableAfterTriggered = false;

    [Header("Events")]
    [SerializeField] private UnityEvent onTriggered;

    private bool triggered;

    private void Reset()
    {
        // Ensure collider is trigger.
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (triggered && oneShot) return;
        if (!other.CompareTag(playerTag)) return;

        UIManager.Instance?.ShowInfo(messageOnTouch);

        triggered = true;
        onTriggered?.Invoke();

        if (disableAfterTriggered)
        {
            gameObject.SetActive(false);
        }
    }
}
