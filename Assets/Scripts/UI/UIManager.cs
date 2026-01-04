using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Behavior")]
    [SerializeField] private bool persistAcrossScenes = false;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("HUD")]
    [SerializeField] private TMP_Text promptText;

    [Header("Toast / Info")]
    [SerializeField] private CanvasGroup toastGroup;
    [SerializeField] private TMP_Text toastText;
    [SerializeField] private float defaultToastDuration = 2.0f;

    [Header("Notice Window (Intro/Instructions/Generic)")]
    [SerializeField] private CanvasGroup noticeGroup;
    [SerializeField] private TMP_Text noticeTitle;
    [SerializeField] private TMP_Text noticeBody;
    [SerializeField] private Button noticeContinueButton;
    [SerializeField] private Button noticeCloseButton;

    [Header("Game Over")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TMP_Text gameOverText;

    /// <summary>
    /// True while the notice/modal window is open.
    /// PlayerInteraction uses this to avoid showing prompts or accepting input.
    /// </summary>
    public bool IsNoticeOpen { get; private set; }

    // Toast queue
    private readonly Queue<(string msg, float duration)> toastQueue = new();
    private Coroutine toastRoutine;

    // Player lock (reuse your existing logic idea)
    [Header("Player Lock While Notice Open")]
    [SerializeField] private bool lockPlayerWhileNoticeOpen = true;
    [SerializeField] private string playerTag = "Player";
    private MonoBehaviour[] cachedPlayerBehaviours;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (persistAcrossScenes)
            DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Hide on start
        if (promptText != null) promptText.gameObject.SetActive(false);

        SetGroupVisible(toastGroup, false, instant: true);
        SetGroupVisible(noticeGroup, false, instant: true);
        IsNoticeOpen = false;

        if (noticeContinueButton != null)
            noticeContinueButton.onClick.AddListener(HideNotice);

        if (noticeCloseButton != null)
            noticeCloseButton.onClick.AddListener(HideNotice);
    }

    // -----------------------------
    // Prompt (existing API)
    // -----------------------------
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

    // -----------------------------
    // Info (existing API) -> now becomes a toast
    // -----------------------------
    public void ShowInfo(string text)
    {
        if (string.IsNullOrWhiteSpace(text)) return;
        EnqueueToast(text, defaultToastDuration);
    }

    public void ClearInfo()
    {
        if (toastText != null) toastText.text = string.Empty;
    }

    public void HideInfo()
    {
        // immediate hide (used when player moves in PlayerInteraction)
        if (toastRoutine != null) StopCoroutine(toastRoutine);
        toastRoutine = null;
        toastQueue.Clear();
        SetGroupVisible(toastGroup, false, instant: false);
    }

    private void EnqueueToast(string msg, float duration)
    {
        if (toastText == null || toastGroup == null)
        {
            // fallback: do nothing
            return;
        }

        toastQueue.Enqueue((msg, duration));
        if (toastRoutine == null)
            toastRoutine = StartCoroutine(ProcessToastQueue());
    }

    private IEnumerator ProcessToastQueue()
    {
        while (toastQueue.Count > 0)
        {
            var (msg, duration) = toastQueue.Dequeue();
            toastText.text = msg;

            yield return Fade(toastGroup, 1f, fadeDuration);

            float t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                yield return null;
            }

            yield return Fade(toastGroup, 0f, fadeDuration);
        }

        toastRoutine = null;
    }

    // -----------------------------
    // Notice window (Intro/Instructions/any modal)
    // -----------------------------
    public void ShowNotice(string title, string body, string continueLabel = "Continue")
    {
        IsNoticeOpen = true;

        // Important: hide HUD prompt so it doesn't overlap the notice content.
        HidePrompt();

        if (noticeTitle != null) noticeTitle.text = title;
        if (noticeBody != null) noticeBody.text = body;

        if (noticeContinueButton != null)
        {
            var t = noticeContinueButton.GetComponentInChildren<TMP_Text>();
            if (t != null) t.text = continueLabel;
        }

        SetGroupVisible(noticeGroup, true, instant: false);

        if (lockPlayerWhileNoticeOpen)
            SetPlayerLocked(true);
    }

    public void HideNotice()
    {
        SetGroupVisible(noticeGroup, false, instant: false);
        IsNoticeOpen = false;

        if (lockPlayerWhileNoticeOpen)
            SetPlayerLocked(false);

        // If the player is still in an interactable trigger, restore the prompt.
        TryRefreshPlayerPrompt();
    }

    private void TryRefreshPlayerPrompt()
    {
        var player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null) return;

        var interaction = player.GetComponent<PlayerInteraction>();
        if (interaction != null)
            interaction.RefreshPrompt();
    }

    private void SetPlayerLocked(bool locked)
    {
        if (cachedPlayerBehaviours == null)
        {
            GameObject player = GameObject.FindGameObjectWithTag(playerTag);
            if (player != null)
                cachedPlayerBehaviours = player.GetComponents<MonoBehaviour>();
        }

        if (cachedPlayerBehaviours == null) return;

        foreach (var b in cachedPlayerBehaviours)
        {
            if (b == null) continue;
            // lock only movement/interaction like you do today
            if (b is PlayerMovement) b.enabled = !locked;
            if (b is PlayerInteraction) b.enabled = !locked;
        }
    }

    // -----------------------------
    // GameOver (existing API)
    // -----------------------------
    public void ShowGameOver(string message)
    {
        if (gameOverText != null) gameOverText.text = message;
        if (gameOverPanel != null) gameOverPanel.SetActive(true);
    }

    public void HideGameOver()
    {
        if (gameOverPanel != null) gameOverPanel.SetActive(false);
    }

    public void RestartLevel()
    {
        HideGameOver();
        Scene current = SceneManager.GetActiveScene();
        SceneManager.LoadScene(current.buildIndex);
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    // -----------------------------
    // CanvasGroup helpers
    // -----------------------------
    private void SetGroupVisible(CanvasGroup group, bool visible, bool instant)
    {
        if (group == null) return;

        group.blocksRaycasts = visible;
        group.interactable = visible;

        if (instant)
        {
            group.alpha = visible ? 1f : 0f;
            return;
        }

        StartCoroutine(Fade(group, visible ? 1f : 0f, fadeDuration));
    }

    private IEnumerator Fade(CanvasGroup group, float target, float duration)
    {
        if (group == null) yield break;

        float start = group.alpha;
        float t = 0f;

        while (t < duration)
        {
            t += Time.unscaledDeltaTime;
            float p = duration <= 0f ? 1f : Mathf.Clamp01(t / duration);
            group.alpha = Mathf.Lerp(start, target, p);
            yield return null;
        }

        group.alpha = target;
    }
}
