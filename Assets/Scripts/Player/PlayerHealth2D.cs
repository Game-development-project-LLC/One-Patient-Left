using UnityEngine;

/// <summary>
/// One-hit death for the player.
/// Any hazard/zombie can call Kill() to end the run.
/// </summary>
public class PlayerHealth2D : MonoBehaviour
{
    [Header("Death Behavior")]
    [SerializeField] private bool disablePlayerOnDeath = true;

    private bool isDead;

    /// <summary>
    /// Kills the player once. Safe to call multiple times.
    /// </summary>
    public void Kill(string reason = "You died!")
    {
        if (isDead) return;
        isDead = true;

        // Show game over UI (if exists)
        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOver(reason);
        }

        if (disablePlayerOnDeath)
        {
            gameObject.SetActive(false);
        }
    }
}
