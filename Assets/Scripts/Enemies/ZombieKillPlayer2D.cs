using UnityEngine;

/// <summary>
/// Simple zombie enemy: if the player touches its collider, the player loses
/// and a Game Over screen is shown.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class ZombieKillPlayer2D : MonoBehaviour
{
    private void Awake()
    {
        // Make sure the collider is a trigger
        var col = GetComponent<Collider2D>();
        col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // We check if the player is the one entering
        var playerMovement = other.GetComponent<PlayerMovement2D>();
        if (playerMovement != null)
        {
            Level1GameManager.Instance?.OnPlayerCaught();
        }
    }
}
