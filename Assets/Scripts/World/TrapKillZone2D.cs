using UnityEngine;

/// <summary>
/// A trigger zone that kills any Killable that enters it (player, zombies, etc.).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public sealed class TrapKillZone2D : MonoBehaviour
{
    private Collider2D col;

    private void Reset()
    {
        col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void Awake()
    {
        col = GetComponent<Collider2D>();
        if (!col.isTrigger)
            col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        var killable = other.GetComponentInParent<Killable>();
        if (killable == null) return;

        // Avoid killing ourselves if misconfigured.
        if (killable.gameObject == gameObject) return;

        killable.Kill(gameObject);
    }
}
