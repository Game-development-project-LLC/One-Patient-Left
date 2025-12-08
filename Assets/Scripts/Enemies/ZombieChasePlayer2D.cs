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

    [Tooltip("If true, will try to copy the player's normal move speed on Awake.")]
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

        // אם לא חיברנו ידנית את השחקן, ננסה למצוא אותו אוטומטית
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
    }

    private void FixedUpdate()
    {
        if (playerTransform == null)
        {
            return; // אין שחקן לתקוף
        }

        Vector2 zombiePosition = rb.position;
        Vector2 playerPosition = playerTransform.position;
        float distanceToPlayer = Vector2.Distance(zombiePosition, playerPosition);

        // התחלת רדיפה
        if (!isChasing && distanceToPlayer <= detectionRadius)
        {
            isChasing = true;
        }

        // הפסקת רדיפה
        if (isChasing && distanceToPlayer >= loseRadius)
        {
            isChasing = false;
        }

        // תנועה רק כשאנחנו במצב רדיפה
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
        // עוזר לראות את רדיוסי הגילוי/איבוד בסצנה
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, loseRadius);
    }
}
