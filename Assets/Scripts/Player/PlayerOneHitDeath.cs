using UnityEngine;

/// <summary>
/// One-hit death for the player. Call Kill() to trigger death.
/// </summary>
public sealed class PlayerOneHitDeath : Killable
{
    [Header("UI")]
    [SerializeField] private GameObject gameOverPanel;

    [Header("Disable On Death")]
    [Tooltip("Scripts that should be disabled when the player dies (movement, interaction, etc.).")]
    [SerializeField] private MonoBehaviour[] disableOnDeath;

    private bool isDead;
    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }

    public override void Kill(GameObject killer)
    {
        if (isDead) return;
        isDead = true;

        if (rb != null)
            rb.linearVelocity = Vector2.zero;

        if (disableOnDeath != null)
        {
            foreach (var script in disableOnDeath)
            {
                if (script != null)
                    script.enabled = false;
            }
        }

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }
}
