using UnityEngine;

/// <summary>
/// One-hit kill zombie health. Can be killed by trap or player attack (tags configurable).
/// </summary>
public class ZombieHealth2D : MonoBehaviour
{
    [Header("Death Settings")]
    [SerializeField] private bool destroyOnDeath = true;
    [SerializeField] private float destroyDelay = 0f;

    [Header("Kill Sources")]
    [SerializeField] private bool killOnTriggerEnter = true;
    [SerializeField] private string[] killTags = { "Trap", "PlayerAttack" };

    [Header("Disable On Death")]
    [SerializeField] private MonoBehaviour[] disableBehaviours;
    [SerializeField] private Collider2D[] disableColliders;
    [SerializeField] private Renderer[] disableRenderers;

    private bool isDead;

    public bool IsDead => isDead;

    public void Kill()
    {
        if (isDead) return;
        isDead = true;

        if (disableBehaviours != null)
        {
            for (int i = 0; i < disableBehaviours.Length; i++)
            {
                if (disableBehaviours[i] != null)
                    disableBehaviours[i].enabled = false;
            }
        }

        if (disableColliders != null)
        {
            for (int i = 0; i < disableColliders.Length; i++)
            {
                if (disableColliders[i] != null)
                    disableColliders[i].enabled = false;
            }
        }

        if (disableRenderers != null)
        {
            for (int i = 0; i < disableRenderers.Length; i++)
            {
                if (disableRenderers[i] != null)
                    disableRenderers[i].enabled = false;
            }
        }

        if (destroyOnDeath)
            Destroy(gameObject, destroyDelay);
    }

    public void TakeHit() => Kill();

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!killOnTriggerEnter || isDead) return;
        if (other == null) return;

        for (int i = 0; i < killTags.Length; i++)
        {
            if (!string.IsNullOrWhiteSpace(killTags[i]) && other.CompareTag(killTags[i]))
            {
                Kill();
                return;
            }
        }
    }
}
