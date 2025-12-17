using UnityEngine;

/// <summary>
/// An interactable switch that triggers a SwitchPanelPuzzle option.
/// </summary>
public class SwitchInteractable : Interactable2D
{
    [Header("Switch Settings")]
    [SerializeField] private SwitchPanelPuzzle panel;
    [SerializeField] private string switchId = "emergency_exit";

    public override void Interact(PlayerInteraction2D player)
    {
        if (panel == null)
        {
            UIManager.Instance?.ShowInfo("Switch is not connected to a panel.");
            return;
        }

        panel.ToggleSwitch(switchId);
    }
}
