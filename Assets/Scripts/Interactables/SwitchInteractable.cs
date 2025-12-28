//using UnityEngine;

//public class SwitchInteractable : Interactable
//{
//    [Header("Switch Settings")]
//    [SerializeField] private SwitchPanelPuzzle panel;
//    [SerializeField] private string switchId;

//    [Header("Behavior")]
//    [Tooltip("If true, this switch disables itself after a successful solve.")]
//    [SerializeField] private bool disableAfterSolved = false;

//    public override void Interact(PlayerInteraction player)
//    {
//        if (panel == null)
//        {
//            Debug.LogWarning("SwitchInteractable: Panel reference is missing.", this);
//            return;
//        }

//        if (string.IsNullOrWhiteSpace(switchId))
//        {
//            Debug.LogWarning("SwitchInteractable: switchId is empty.", this);
//            return;
//        }

//        panel.ToggleSwitch(switchId);

//        if (disableAfterSolved && panel.IsSolved)
//            gameObject.SetActive(false);
//    }

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        if (panel == null)
//        {
//            // Try to auto-find a panel in parents (nice QoL)
//            panel = GetComponentInParent<SwitchPanelPuzzle>();
//        }

//        if (panel != null && !string.IsNullOrWhiteSpace(switchId))
//        {
//            // No hard validation against puzzle list here (keeps this file independent),
//            // but you can add custom checks if you want.
//        }
//    }
//#endif
//}
