using UnityEngine;

/// <summary>
/// Simple zombie enemy: if the player touches its collider, the player loses
/// and a Game Over screen is shown.
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
        // חשוב: הקוליידר *לא* טריגר, כדי שהזומבי ייתקע בקירות
        var col = GetComponent<Collider2D>();
        col.isTrigger = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // נבדוק אם שחקן נכנס בהתנגשות
        var playerMovement = collision.gameObject.GetComponent<PlayerMovement2D>();
        if (playerMovement == null)
        {
            return;
        }

        // מכבים שליטה בשחקן
        playerMovement.enabled = false;

        // מציגים מסך Game Over
        UIManager.Instance?.ShowGameOver(gameOverMessage);
    }
}
