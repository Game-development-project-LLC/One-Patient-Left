using UnityEngine;

/// <summary>
/// Shows an instructions panel at the start of the level,
/// and lets the player toggle it with a key.
/// </summary>
public class InstructionsController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject instructionsPanel;
    [SerializeField] private PlayerMovement2D playerMovement;

    [Header("Behaviour")]
    [SerializeField] private bool showOnStart = true;

    [Tooltip("Keyboard key to toggle the instructions panel.")]
    [SerializeField] private KeyCode toggleKey = KeyCode.H;

    private bool isVisible;

    private void Start()
    {
        if (instructionsPanel == null)
        {
            return;
        }

        if (showOnStart)
        {
            ShowInstructions();
        }
        else
        {
            instructionsPanel.SetActive(false);
            isVisible = false;
        }
    }

    private void Update()
    {
        if (instructionsPanel == null)
        {
            return;
        }

        if (Input.GetKeyDown(toggleKey))
        {
            if (isVisible)
            {
                HideInstructions();
            }
            else
            {
                ShowInstructions();
            }
        }
    }

    public void ShowInstructions()
    {
        instructionsPanel.SetActive(true);
        isVisible = true;

        // Disable player movement while reading instructions
        if (playerMovement != null)
        {
            playerMovement.enabled = false;
        }
    }

    public void HideInstructions()
    {
        instructionsPanel.SetActive(false);
        isVisible = false;

        // Re-enable player movement
        if (playerMovement != null)
        {
            playerMovement.enabled = true;
        }
    }
}
