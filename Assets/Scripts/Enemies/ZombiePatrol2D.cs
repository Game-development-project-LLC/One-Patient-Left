using UnityEngine;

/// <summary>
/// Patrols between waypoints and chases the player when detected.
/// Player is not detected while PlayerStealthState.IsStealth is true.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public sealed class ZombiePatrol2D : MonoBehaviour
{
    [Header("Patrol")]
    [SerializeField] private Transform[] waypoints;
    [SerializeField] private float patrolSpeed = 1.5f;
    [SerializeField] private float arriveDistance = 0.1f;

    [Header("Chase")]
    [SerializeField] private float detectionRange = 2.5f;
    [SerializeField] private float chaseSpeed = 2.2f;
    [SerializeField] private float loseRangeMultiplier = 1.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private string playerTag = "Player";

    private Rigidbody2D rb;
    private int waypointIndex;
    private Transform playerTarget;
    private Vector2 lastKnownPlayerPos;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.gravityScale = 0f;
        rb.freezeRotation = true;
    }

    private void FixedUpdate()
    {
        UpdateTarget();

        if (playerTarget != null)
        {
            MoveTowards((Vector2)playerTarget.position, chaseSpeed);
            lastKnownPlayerPos = playerTarget.position;
            return;
        }

        Patrol();
    }

    private void UpdateTarget()
    {
        // If currently chasing, validate target
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

        // Acquire new target
        Collider2D hit = Physics2D.OverlapCircle(transform.position, detectionRange, playerLayer);
        if (hit == null) return;
        if (!hit.CompareTag(playerTag)) return;

        Transform candidate = hit.transform;
        if (IsPlayerHidden(candidate)) return;

        playerTarget = candidate;
        lastKnownPlayerPos = candidate.position;
    }

    private bool IsPlayerHidden(Transform player)
    {
        var stealth = player.GetComponentInParent<PlayerStealthState>();
        return stealth != null && stealth.IsStealth;
    }

    private void Patrol()
    {
        if (waypoints == null || waypoints.Length == 0 || waypoints[waypointIndex] == null)
            return;

        Vector2 target = waypoints[waypointIndex].position;
        Vector2 pos = rb.position;
        Vector2 dir = target - pos;

        if (dir.magnitude <= arriveDistance)
        {
            waypointIndex = (waypointIndex + 1) % waypoints.Length;
            return;
        }

        MoveTowards(target, patrolSpeed);
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
