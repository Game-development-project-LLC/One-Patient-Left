using UnityEngine;

/// <summary>
/// Kills the player on contact. Also makes the zombie killable (one-hit).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public sealed class ZombieKillPlayer2D : Killable
{
    [Header("Player")]
    [SerializeField] private string playerTag = "Player";

    [Header("Zombie Death")]
    [SerializeField] private bool destroyOnKill = true;
    [SerializeField] private GameObject disableRootOnKill;

    private bool isDead;

    private void Awake()
    {
        if (disableRootOnKill == null)
            disableRootOnKill = gameObject;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        TryKillPlayer(collision.collider);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        TryKillPlayer(other);
    }

    private void TryKillPlayer(Collider2D other)
    {
        if (isDead) return;
        if (other == null) return;
        if (!other.CompareTag(playerTag)) return;

        var playerDeath = other.GetComponentInParent<PlayerOneHitDeath>();
        if (playerDeath != null)
            playerDeath.Kill(gameObject);
    }

    public override void Kill(GameObject killer)
    {
        if (isDead) return;
        isDead = true;

        if (destroyOnKill)
        {
            Destroy(gameObject);
            return;
        }

        if (disableRootOnKill != null)
            disableRootOnKill.SetActive(false);
    }
}
