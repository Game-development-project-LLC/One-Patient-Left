using UnityEngine;
using TMPro;

/// <summary>
/// Handles simple UI: interaction prompt and info messages.
/// </summary>
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("UI References")]
    public TMP_Text promptText;
    public TMP_Text infoText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        // For now we stay only in this scene, no need for DontDestroyOnLoad.
    }

    private void Start()
    {
        HidePrompt();
        ClearInfo();
    }

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
}
