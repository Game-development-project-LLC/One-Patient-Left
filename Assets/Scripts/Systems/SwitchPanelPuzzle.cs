//using System.Collections.Generic;
//using UnityEngine;

//public class SwitchPanelPuzzle : MonoBehaviour
//{
//    [System.Serializable]
//    public class SwitchOption
//    {
//        [Tooltip("Unique switch identifier used by SwitchInteractable.")]
//        public string id;

//        [Tooltip("True only for the correct switch.")]
//        public bool isCorrect;
//    }

//    [Header("Puzzle Setup")]
//    [SerializeField] private SwitchOption[] switches;
//    [SerializeField] private ExitDoorInteractable doorToUnlock;

//    [Header("Fail Consequence")]
//    [SerializeField] private GameObject zombieToEnableOnFail;
//    [SerializeField] private string failMessage = "Wrong switch. You made noise...";

//    [Header("Success")]
//    [SerializeField] private string successMessage = "Power routed correctly. The exit is now unlocked.";

//    [Header("Behavior")]
//    [Tooltip("If true, wrong switches still work after the puzzle is solved (usually false).")]
//    [SerializeField] private bool allowInteractionsAfterSolved = false;

//    private bool solved;
//    private Dictionary<string, bool> lookup; // id -> isCorrect

//    private void Awake()
//    {
//        BuildLookup();
//    }

//    private void OnEnable()
//    {
//        // In case values changed during edit-time and domain reload didn't rebuild properly.
//        if (lookup == null || lookup.Count == 0)
//            BuildLookup();
//    }

//    public bool IsSolved => solved;

//    public void ToggleSwitch(string switchId)
//    {
//        if (solved && !allowInteractionsAfterSolved)
//            return;

//        if (string.IsNullOrWhiteSpace(switchId))
//        {
//            UIManager.Instance?.ShowInfo("Unknown switch.");
//            return;
//        }

//        if (lookup == null || lookup.Count == 0)
//            BuildLookup();

//        if (!lookup.TryGetValue(switchId, out bool isCorrect))
//        {
//            UIManager.Instance?.ShowInfo("Unknown switch.");
//            return;
//        }

//        if (isCorrect)
//            HandleSuccess();
//        else
//            HandleFail();
//    }

//    private void HandleSuccess()
//    {
//        if (solved)
//        {
//            UIManager.Instance?.ShowInfo(successMessage);
//            return;
//        }

//        solved = true;

//        if (doorToUnlock != null)
//            doorToUnlock.SetUnlocked(true);

//        UIManager.Instance?.ShowInfo(successMessage);
//    }

//    private void HandleFail()
//    {
//        UIManager.Instance?.ShowInfo(failMessage);

//        if (zombieToEnableOnFail != null)
//            zombieToEnableOnFail.SetActive(true);
//    }

//    private void BuildLookup()
//    {
//        lookup = new Dictionary<string, bool>();

//        if (switches == null)
//            return;

//        for (int i = 0; i < switches.Length; i++)
//        {
//            SwitchOption s = switches[i];
//            if (s == null) continue;

//            if (string.IsNullOrWhiteSpace(s.id))
//                continue;

//            // If duplicates exist, the last one wins (we also warn in OnValidate).
//            lookup[s.id] = s.isCorrect;
//        }
//    }

//#if UNITY_EDITOR
//    private void OnValidate()
//    {
//        // Validate data quality in the editor
//        if (switches == null || switches.Length == 0)
//            return;

//        HashSet<string> seen = new HashSet<string>();
//        int correctCount = 0;

//        for (int i = 0; i < switches.Length; i++)
//        {
//            SwitchOption s = switches[i];
//            if (s == null)
//            {
//                Debug.LogWarning($"SwitchPanelPuzzle: Null switch entry at index {i}.", this);
//                continue;
//            }

//            if (string.IsNullOrWhiteSpace(s.id))
//            {
//                Debug.LogWarning($"SwitchPanelPuzzle: Switch at index {i} has an empty id.", this);
//                continue;
//            }

//            if (!seen.Add(s.id))
//                Debug.LogWarning($"SwitchPanelPuzzle: Duplicate switch id '{s.id}'. ids must be unique.", this);

//            if (s.isCorrect)
//                correctCount++;
//        }

//        if (correctCount == 0)
//            Debug.LogWarning("SwitchPanelPuzzle: No correct switch is marked. Puzzle cannot be solved.", this);

//        if (correctCount > 1)
//            Debug.LogWarning("SwitchPanelPuzzle: More than one switch is marked correct. Usually you want exactly one.", this);

//        // Keep lookup in sync during edit-time changes (useful for Play Mode without domain reload).
//        BuildLookup();
//    }
//#endif
//}
