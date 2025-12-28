using UnityEngine;

/// <summary>
/// Stays near its home position, chases the player when detected.
/// Player is not detected while PlayerStealthState.IsStealth is true.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public sealed class ZombieChasePlayer2D : MonoBehaviour
{
    [Header("Home")]
    [SerializeField] private float returnSpeed = 1.4f;

    [Header("Chase")]
    [SerializeField] private float detectionRange = 2.5f;
    [SerializeField] private float chaseSpeed = 2.2f;
    [SerializeField] private float loseRangeMultiplier = 1.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string playerTag = "Player";

    private Rigidbody2D rb;
    private Transform playerTarget;
    private Vector2 homePos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        homePos = rb.position;
    }

    private void FixedUpdate()
    {
        UpdateTarget();

        if (playerTarget != null)
        {
            MoveTowards(playerTarget.position, chaseSpeed);
            return;
        }

        // Return home
        MoveTowards(homePos, returnSpeed);
    }

    private void UpdateTarget()
    {
        if (playerTarget != null)
        {
            if (IsPlayerHidden(playerTarget))
            {
                playerTarget = null;
                return;
            }

            float maxDist = detectionRange * loseRangeMultiplier;
            if (Vector2.Distance(transform.position, playerTarget.position) > maxDist)
                playerTarget = null;

            return;
        }

        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (hit == null) return;
        if (!hit.CompareTag(playerTag)) return;

        Transform candidate = hit.transform;
        if (IsPlayerHidden(candidate)) return;

        playerTarget = candidate;
    }

    private bool IsPlayerHidden(Transform player)
    {
        var stealth = player.GetComponentInParent<PlayerStealthState>();
        return stealth != null && stealth.IsStealth;
    }

    private void MoveTowards(Vector2 target, float speed)
    {
        Vector2 pos = rb.position;
        Vector2 dir = target - pos;

        if (dir.sqrMagnitude < 0.0001f) return;

        Vector2 next = pos + dir.normalized * speed * Time.fixedDeltaTime;
        rb.MovePosition(next);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, detectionRange);
    }
}
