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

    [Tooltip("Which layers can be detected as the player. Set to Everything if unsure.")]
    [SerializeField] private LayerMask playerLayer = ~0;

    [SerializeField] private string playerTag = "Player";

    [Tooltip("If the player's collider is Trigger, enable this so the zombie can still detect them.")]
    [SerializeField] private bool detectTriggerColliders = true;

    private Rigidbody2D rb;
    private Transform playerTarget;
    private Vector2 homePos;

    // Non-alloc buffer to avoid GC
    private readonly Collider2D[] hits = new Collider2D[16];
    private ContactFilter2D filter;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;

        homePos = rb.position;

        // Build filter (fallback to Everything if mask is "Nothing")
        LayerMask maskToUse = (playerLayer.value == 0) ? (LayerMask)~0 : playerLayer;

        filter = new ContactFilter2D
        {
            useLayerMask = true,
            layerMask = maskToUse,
            useTriggers = detectTriggerColliders
        };
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
        // Keep current target if still valid
        if (playerTarget != null)
        {
            if (IsPlayerHidden(playerTarget))
            {
                playerTarget = null;
                return;
            }

            float maxDist = detectionRange * loseRangeMultiplier;
            if (Vector2.Distance(rb.position, (Vector2)playerTarget.position) > maxDist)
                playerTarget = null;

            return;
        }

        // Acquire new target
        int count = Physics2D.OverlapCircle(rb.position, detectionRange, filter, hits);
        if (count <= 0) return;

        Transform best = null;
        float bestSqr = float.PositiveInfinity;

        for (int i = 0; i < count; i++)
        {
            Collider2D c = hits[i];
            if (c == null) continue;
            if (!c.CompareTag(playerTag)) continue;

            Transform candidate = c.transform;

            if (IsPlayerHidden(candidate)) continue;

            float sqr = ((Vector2)candidate.position - rb.position).sqrMagnitude;
            if (sqr < bestSqr)
            {
                bestSqr = sqr;
                best = candidate;
            }
        }

        playerTarget = best;
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
