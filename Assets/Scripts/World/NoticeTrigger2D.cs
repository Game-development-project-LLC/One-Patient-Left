using UnityEngine;

/// <summary>
/// Put this on a GameObject with a 2D trigger collider.
/// When the player enters, it shows the NoticeWindow (and can pause the game).
/// </summary>
public class NoticeTrigger2D : MonoBehaviour
{
    [Header("UI Text")]
    public string title = "Notice";

    [TextArea(4, 12)]
    public string body = "Some text...";

    [Header("Behavior")]
    public bool pauseGame = true;
    public bool triggerOnce = true;
    public string continueLabel = "Continue";

    private bool _used;

    private void Reset()
    {
        var col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (_used) return;
        if (!other.CompareTag("Player")) return;

        if (UIManager.Instance != null)
            UIManager.Instance.ShowNotice(title, body, pauseGame, continueLabel);

        if (triggerOnce)
            _used = true;
    }
}
