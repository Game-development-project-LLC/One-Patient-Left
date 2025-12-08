using UnityEngine;

/// <summary>
/// Zombie that stands still until the player is inside a detection radius,
/// then chases the player using a constant move speed.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
public class ZombieChasePlayer2D : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 4f;

    [Tooltip("If true, copies the player's normal move speed on Awake when found.")]
    [SerializeField] private bool matchPlayerSpeed = true;

    [Header("Detection")]
    [SerializeField] private float detectionRadius = 5f;
    [SerializeField] private float loseRadius = 7f;

    [Header("References")]
    [SerializeField] private Transform playerTransform = null;

    private Rigidbody2D rb;
    private bool isChasing = false;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

        // If the player reference is not set manually, try to find it automatically.
#if UNITY_2023_1_OR_NEWER
        if (playerTransform == null)
        {
            var player = Object.FindAnyObjectByType<PlayerMovement2D>();
            if (player != null)
            {
                playerTransform = player.transform;

                if (matchPlayerSpeed)
                {
                    moveSpeed = player.getNormalSpeed();
                }
            }
        }
#else
        if (playerTransform == null)
        {
            var player = FindObjectOfType<PlayerMovement2D>();
            if (player != null)
            {
                playerTransform = player.transform;

                if (matchPlayerSpeed)
                {
                    moveSpeed = player.getNormalSpeed();
                }
            }
        }
#endif
    }

    private void FixedUpdate()
    {
        if (playerTransform == null)
        {
            return; // no player to chase
        }

        Vector2 zombiePosition = rb.position;
        Vector2 playerPosition = playerTransform.position;
        float distanceToPlayer = Vector2.Distance(zombiePosition, playerPosition);

        // Start chasing
        if (!isChasing && distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
        }

        // Stop chasing
        if (isChasing && distanceToPlayer >= loseRadius)
        {
            isChasing = false;
        }

        // Move only while chasing
        if (isChasing)
        {
            Vector2 direction = (playerPosition - zombiePosition).normalized;
            Vector2 targetPosition =
                zombiePosition + direction * moveSpeed * Time.fixedDeltaTime;

            rb.MovePosition(targetPosition);
        }
    }

    private void OnDrawGizmosSelected()
    {
        // Visualize detection and lose radii in the Scene view
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRadius);
    }
}
