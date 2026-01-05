using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Behavior")]
    [SerializeField] private bool persistAcrossScenes = false;
    [SerializeField] private float fadeDuration = 0.5f;

    [Header("Notice Keyboard")]
    [SerializeField] private bool closeNoticeWithKeyboard = true;
    [SerializeField] private bool anyKeyClosesNotice = false;

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

    [Header("Game Over Messages By Reason")]
    [TextArea(2, 6)]
    [SerializeField]
    private string gameOverDefaultMessage =
        "Your journey ends here. The hospital keeps its secrets, and you become one more silent room in its endless hallways.";

    [TextArea(2, 6)]
    [SerializeField]
    private string gameOverZombieMessage =
        "You feel a wet breath at your neck. Fingers like ice close around you, and the corridor swallows your last scream.";

    [TextArea(2, 6)]
    [SerializeField]
    private string gameOverTrapMessage =
        "One careless step. The floor gives way, and the world drops out from under you—metal, darkness, and silence.";

    // Toast queue
    private readonly Queue<(string msg, float duration)> toastQueue = new();
    private Coroutine toastRoutine;

    // Player lock (reuse your existing logic idea)
    [Header("Player Lock While Notice Open")]
    [SerializeField] private bool lockPlayerWhileNoticeOpen = true;
    [SerializeField] private string playerTag = "Player";
    private MonoBehaviour[] cachedPlayerBehaviours;

    // Pause whole game while notice is open (optional)
    private bool _pausedByNotice = false;
    private float _prevTimeScale = 1f;

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

        if (noticeContinueButton != null)
            noticeContinueButton.onClick.AddListener(HideNotice);

        if (noticeCloseButton != null)
            noticeCloseButton.onClick.AddListener(HideNotice);
    }

    private void Update()
    {
        if (!closeNoticeWithKeyboard) return;
        if (!IsNoticeOpen) return;

        if (anyKeyClosesNotice)
        {
            if (WasAnyKeyPressedThisFrame())
            {
                HideNotice();
            }
            return;
        }

        if (WasNoticeCloseKeyPressedThisFrame())
            HideNotice();
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
            return;

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
    // Old signature (kept!) - 3rd parameter is continue button label.
    public void ShowNotice(string title, string body, string continueLabel = "Continue")
    {
        ShowNoticeInternal(title, body, continueLabel, pauseGame: false);
    }

    // New overloads - allow passing "pauseGame" as 3rd argument.
    public void ShowNotice(string title, string body, bool pauseGame)
    {
        ShowNoticeInternal(title, body, continueLabel: "Continue", pauseGame: pauseGame);
    }

    public void ShowNotice(string title, string body, bool pauseGame, string continueLabel)
    {
        ShowNoticeInternal(title, body, continueLabel, pauseGame);
    }

    private void ShowNoticeInternal(string title, string body, string continueLabel, bool pauseGame)
    {
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

        if (pauseGame)
            PauseByNotice();
    }

    public void HideNotice()
    {
        SetGroupVisible(noticeGroup, false, instant: false);

        if (lockPlayerWhileNoticeOpen)
            SetPlayerLocked(false);

        ResumeIfPausedByNotice();
    }

    private void PauseByNotice()
    {
        if (_pausedByNotice) return;
        _pausedByNotice = true;
        _prevTimeScale = Time.timeScale;
        Time.timeScale = 0f;
    }

    private void ResumeIfPausedByNotice()
    {
        if (!_pausedByNotice) return;
        _pausedByNotice = false;
        Time.timeScale = _prevTimeScale;
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
            if (b is PlayerMovement) b.enabled = !locked;
            if (b is PlayerInteraction) b.enabled = !locked;
        }
    }

    // -----------------------------
    // GameOver
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

    public bool IsNoticeOpen => noticeGroup != null && noticeGroup.alpha > 0.001f;

    public string GetGameOverMessage(DeathReason reason)
    {
        string fallback = string.IsNullOrEmpty(gameOverDefaultMessage) ? "Game Over." : gameOverDefaultMessage;

        switch (reason)
        {
            case DeathReason.Zombie:
                return string.IsNullOrEmpty(gameOverZombieMessage) ? fallback : gameOverZombieMessage;

            case DeathReason.Trap:
                return string.IsNullOrEmpty(gameOverTrapMessage) ? fallback : gameOverTrapMessage;

            default:
                return fallback;
        }
    }

    public void ShowGameOver(DeathReason reason)
    {
        ShowGameOver(GetGameOverMessage(reason));
    }

    private bool WasNoticeCloseKeyPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        var kb = Keyboard.current;
        if (kb != null)
        {
            if (kb.enterKey.wasPressedThisFrame) return true;
            if (kb.numpadEnterKey.wasPressedThisFrame) return true;
            if (kb.spaceKey.wasPressedThisFrame) return true;
            if (kb.escapeKey.wasPressedThisFrame) return true;
            if (kb.eKey.wasPressedThisFrame) return true;
        }
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.GetKeyDown(KeyCode.Return)) return true;
        if (Input.GetKeyDown(KeyCode.KeypadEnter)) return true;
        if (Input.GetKeyDown(KeyCode.Space)) return true;
        if (Input.GetKeyDown(KeyCode.Escape)) return true;
        if (Input.GetKeyDown(KeyCode.E)) return true;
#endif
        return false;
    }

    private bool WasAnyKeyPressedThisFrame()
    {
#if ENABLE_INPUT_SYSTEM
        if (Keyboard.current != null && Keyboard.current.anyKey.wasPressedThisFrame)
            return true;
#endif

#if ENABLE_LEGACY_INPUT_MANAGER
        if (Input.anyKeyDown) return true;
#endif
        return false;
    }
}
