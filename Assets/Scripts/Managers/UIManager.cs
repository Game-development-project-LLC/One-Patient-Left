using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Handles simple UI: interaction prompt, info messages and game over panel.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private TMP_Text promptText;
    [SerializeField] private TMP_Text infoText;

    [Header("Game Over UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        HidePrompt();
        ClearInfo();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    // ===== Prompt =====
    public void ShowPrompt(string text)
    {
        if (promptText == null)
        {
            return;
        }

        promptText.text = text;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText == null)
        {
            return;
        }

        promptText.text = string.Empty;
        promptText.gameObject.SetActive(false);
    }

    // ===== Info =====
    public void ShowInfo(string text)
    {
        if (infoText == null)
        {
            return;
        }

        infoText.text = text;
        infoText.gameObject.SetActive(true);
    }

    public void ClearInfo()
    {
        if (infoText == null)
        {
            return;
        }

        infoText.text = string.Empty;
    }

    // ===== Game Over =====
    public void ShowGameOver(string message)
    {
        if (gameOverText != null)
        {
            gameOverText.text = message;
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    /// <summary>
    /// Reloads the currently active scene. Hook this to the Restart button.
    /// </summary>
    public void RestartLevel()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }
}
