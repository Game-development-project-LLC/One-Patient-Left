using UnityEngine;

/// <summary>
/// One-hit death trigger for the player. Other hazards call Kill(gameObject).
/// This script infers the death reason (Zombie / Trap) from the killer object,
/// then asks UIManager for the correct Game Over message.
/// </summary>
public sealed class PlayerOneHitDeath : Killable
{
    [Header("Optional References")]
    [SerializeField] private PlayerHealth2D health;               // If null, will auto-find on this GameObject.
    [SerializeField] private GameObject fallbackGameOverPanel;     // Used only if UIManager/Health aren't available.

    [Header("Disable On Death (Optional)")]
    [SerializeField] private MonoBehaviour[] disableOnDeath;

    private bool isDead;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        if (health == null)
            health = GetComponent<PlayerHealth2D>();

        if (fallbackGameOverPanel != null)
            fallbackGameOverPanel.SetActive(false);
    }

    public override void Kill(GameObject killer)
    {
        if (isDead) return;
        isDead = true;

        // Stop player motion immediately (if present).
        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        // Disable any scripts you don't want running after death (movement, interaction, etc.).
        if (disableOnDeath != null)
        {
            foreach (var script in disableOnDeath)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        DeathReason reason = InferReason(killer);

        // Preferred flow: let PlayerHealth2D handle death logic (it already calls UIManager.ShowGameOver(string)).
        if (health != null)
        {
            string msg = (UIManager.Instance != null) ? UIManager.Instance.GetGameOverMessage(reason) : DefaultMessage(reason);
            health.Kill(msg);
            return;
        }

        // Fallback: show directly through UIManager if health isn't present.
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(reason);
            return;
        }

        // Last fallback: just enable a local panel.
        if (fallbackGameOverPanel != null)
            fallbackGameOverPanel.SetActive(true);
    }

    private static DeathReason InferReason(GameObject killer)
    {
        if (killer == null) return DeathReason.Unknown;

        // If the killer is (or is under) a zombie object:
        if (killer.GetComponentInParent<ZombieKillPlayer2D>() != null)
            return DeathReason.Zombie;

        // If the killer is (or is under) a trap object:
        if (killer.GetComponentInParent<TrapKillZone2D>() != null)
            return DeathReason.Trap;

        return DeathReason.Unknown;
    }

    private static string DefaultMessage(DeathReason reason)
    {
        switch (reason)
        {
            case DeathReason.Zombie:
                return "You were caught.";
            case DeathReason.Trap:
                return "You fell into a trap.";
            default:
                return "Game Over.";
        }
    }
}
