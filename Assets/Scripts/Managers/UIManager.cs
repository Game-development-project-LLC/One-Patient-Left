using UnityEngine;
using TMPro;

/// <summary>
/// Handles simple UI: interaction prompt, info messages and game over panel.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    public TMP_Text promptText;
    public TMP_Text infoText;

    [Header("Game Over UI")]
    public GameObject gameOverPanel;   // panel with dark background
    public TMP_Text gameOverText;      // text inside the panel

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
            gameOverPanel.SetActive(false);   // make sure it's hidden at start
    }

    // ===== Prompt =====
    public void ShowPrompt(string text)
    {
        if (promptText == null) return;
        promptText.text = text;
        promptText.gameObject.SetActive(true);
    }

    public void HidePrompt()
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(false);
    }

    // ===== Info =====
    public void ShowInfo(string text)
    {
        if (infoText == null) return;
        infoText.text = text;
        infoText.gameObject.SetActive(true);
    }

    public void ClearInfo()
    {
        if (infoText == null) return;
        infoText.text = "";
    }

    // ===== Game Over =====
    public void ShowGameOver(string message)
    {
        if (gameOverText != null)
            gameOverText.text = message;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
    }
}
