using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Simple UI manager for prompt/info/game over.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Behavior")]
    [SerializeField] private bool persistAcrossScenes = false;

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

        if (persistAcrossScenes)
        {
            DontDestroyOnLoad(gameObject);
        }

        HidePrompt();
        HideInfo();
        HideGameOver();
    }

    // ---------------- Prompt ----------------

    public void ShowPrompt(string text)
    {
        if (promptText == null) return;
        promptText.gameObject.SetActive(true);
        promptText.text = text;
    }

    public void HidePrompt()
    {
        if (promptText == null) return;
        promptText.text = string.Empty;
        promptText.gameObject.SetActive(false);
    }

    // ---------------- Info ----------------

    public void ShowInfo(string text)
    {
        if (infoText == null) return;
        infoText.gameObject.SetActive(true);
        infoText.text = text;
    }

    public void ClearInfo()
    {
        if (infoText == null) return;
        infoText.text = string.Empty;
    }

    // Backwards-compatible name (your PlayerInteraction expects this)
    public void HideInfo()
    {
        if (infoText == null) return;
        infoText.text = string.Empty;
        infoText.gameObject.SetActive(false);
    }

    // ---------------- Game Over ----------------

    public void ShowGameOver(string message)
    {
        if (gameOverText != null) gameOverText.text = message;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    // ---------------- Buttons ----------------

    /// <summary>
    /// Called by the Restart button.
    /// Reloads the active scene and resets time scale.
    /// </summary>
    public void RestartLevel()
    {
        Time.timeScale = 1f;
        HidePrompt();
        HideInfo();
        HideGameOver();

        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    /// <summary>
    /// Optional: quit play mode or application.
    /// </summary>
    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
