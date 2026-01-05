using UnityEngine;
using UnityEngine.InputSystem;

public class InstructionsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject instructionsPanel;

    [Header("Input (New Input System)")]
    [SerializeField] private InputActionReference toggleInstructionsAction;

    [Header("Behaviour")]
    [SerializeField] private bool showOnStart = true;

    [Header("Player Lock")]
    [Tooltip("Tag used to find the player GameObject.")]
    [SerializeField] private string playerTag = "Player";

    private bool isVisible;
    private MonoBehaviour[] cachedPlayerBehaviours;

    private void OnEnable()
    {
        if (toggleInstructionsAction != null)
        {
            toggleInstructionsAction.action.Enable();
            toggleInstructionsAction.action.performed += OnTogglePerformed;
        }
    }

    private void OnDisable()
    {
        if (toggleInstructionsAction != null)
        {
            toggleInstructionsAction.action.performed -= OnTogglePerformed;
            toggleInstructionsAction.action.Disable();
        }
    }

    private void Start()
    {
        CachePlayerBehaviours();

        if (showOnStart) ShowInstructions();
        else HideInstructions();
    }

    private void CachePlayerBehaviours()
    {
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);
        if (player == null)
        {
            Debug.LogWarning($"InstructionsController: No GameObject with tag '{playerTag}' found.");
            cachedPlayerBehaviours = null;
            return;
        }

        // Disable/enable all scripts on the player EXCEPT inventory/UI/etc.
        cachedPlayerBehaviours = player.GetComponents<MonoBehaviour>();
    }

    private void OnTogglePerformed(InputAction.CallbackContext ctx)
    {
        Debug.Log("ToggleInstructions pressed");
        if (instructionsPanel == null)
            return;

        if (isVisible) HideInstructions();
        else ShowInstructions();
    }

    public void ShowInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(true);

        isVisible = true;

        UIManager.Instance?.HidePrompt();

        SetPlayerInputEnabled(false);
    }

    public void HideInstructions()
    {
        if (instructionsPanel != null)
            instructionsPanel.SetActive(false);

        isVisible = false;

        UIManager.Instance?.HideInfo();

        SetPlayerInputEnabled(true);
    }

    private void SetPlayerInputEnabled(bool enabled)
    {
        if (cachedPlayerBehaviours == null || cachedPlayerBehaviours.Length == 0)
            CachePlayerBehaviours();

        if (cachedPlayerBehaviours == null)
            return;

        foreach (MonoBehaviour b in cachedPlayerBehaviours)
        {
            if (b == null) continue;

            // Whitelist: do NOT disable UI / Inventory / Interaction controller if you still want them
            // while instructions are open. If you want to freeze everything, remove this block.
            if (b is PlayerMovement) { b.enabled = enabled; continue; }
            if (b is PlayerInteraction) { b.enabled = enabled; continue; }

            // Add more scripts here if needed:
            // if (b is PlayerInventory) { b.enabled = enabled; continue; }
        }
    }
}
