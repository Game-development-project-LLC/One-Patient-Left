using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Keeps simple level state for Level 1: whether the player has the keycard,
/// and whether the player picked up the photo.
/// </summary>
public class Level1GameManager : MonoBehaviour
{
    public static Level1GameManager Instance { get; private set; }

    public bool HasKeycard { get; private set; }
    public bool HasPhoto { get; private set; }
    private bool isGameOver = false;   // NEW

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void GiveKeycard()
    {
        HasKeycard = true;
        Debug.Log("Player obtained keycard.");
    }

    public void GivePhoto()                      // NEW
    {
        HasPhoto = true;
        Debug.Log("Player picked up the photo.");
    }

    // NEW: called when the zombie catches the player
    public void OnPlayerCaught()
    {
        if (isGameOver) return;

        isGameOver = true;
        Debug.Log("Player was caught by a zombie.");

        // disable player movement & interaction
        var player = FindObjectOfType<PlayerMovement2D>();
        if (player != null)
            player.enabled = false;

        var interaction = FindObjectOfType<PlayerInteraction2D>();
        if (interaction != null)
            interaction.enabled = false;

        UIManager.Instance?.ShowGameOver("You were caught.\nPress Restart to try again.");
    }

    // NEW: restart level, hooked to the Restart button
    public void RestartLevel()
    {
        // Simple reload of current scene
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
