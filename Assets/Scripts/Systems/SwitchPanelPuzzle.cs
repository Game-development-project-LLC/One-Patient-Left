using UnityEngine;

/// <summary>
/// Simple switch panel puzzle.
/// One switch is correct, others cause a negative consequence.
/// Used for emergency exit and apartment power panels.
/// </summary>
public class SwitchPanelPuzzle : MonoBehaviour
{
    [System.Serializable]
    public class SwitchOption
    {
        public string id;       // Unique switch identifier
        public bool isCorrect;  // True only for the correct switch
    }

    [Header("Puzzle Setup")]
    [SerializeField] private SwitchOption[] switches;
    [SerializeField] private ExitDoorInteractable doorToUnlock;
    [SerializeField] private UIManager ui;

    [Header("Fail Consequence")]
    [SerializeField] private GameObject zombieToEnableOnFail;
    [SerializeField] private string failMessage = "Wrong switch. You made noise...";

    [Header("Success")]
    [SerializeField] private string successMessage = "Power routed correctly.";

    private bool solved = false;

    public void ToggleSwitch(string switchId)
    {
        if (solved)
            return;

        for (int i = 0; i < switches.Length; i++)
        {
            if (switches[i].id != switchId)
                continue;

            if (switches[i].isCorrect)
            {
                solved = true;

                if (doorToUnlock != null)
                    doorToUnlock.SetUnlocked(true);

                if (ui != null)
                    ui.ShowInfo(successMessage);
            }
            else
            {
                if (ui != null)
                    ui.ShowInfo(failMessage);

                if (zombieToEnableOnFail != null)
                    zombieToEnableOnFail.SetActive(true);
            }

            return;
        }

        if (ui != null)
            ui.ShowInfo("Unknown switch.");
    }
}
