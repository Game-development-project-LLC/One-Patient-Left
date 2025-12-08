using UnityEngine;

/// <summary>
/// Simple zombie enemy: if the player touches its collider,
/// the player loses and a Game Over screen is shown.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class ZombieKillPlayer2D : MonoBehaviour
{
    [Header("Game Over")]
    [TextArea]
    [SerializeField] private string gameOverMessage = "The zombie caught you.";

    private void Awake()
    {
        // The collider must NOT be a trigger so the zombie collides with walls.
        var col = GetComponent<Collider2D>();
        col.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the player entered the collision
        var playerMovement = collision.gameObject.GetComponent<PlayerMovement2D>();
        if (playerMovement == null)
        {
            return;
        }

        // Disable player movement
        playerMovement.enabled = false;

        // Show Game Over screen
        UIManager.Instance?.ShowGameOver(gameOverMessage);
    }
}
