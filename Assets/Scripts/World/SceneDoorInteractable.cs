using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.SceneManagement;
using Services;

/// <summary>
/// Interactable door:
/// - If requiredItemId is set: player must have it.
/// - If allowed: AUTO-SAVES then loads target scene (by name).
///
/// Auto-save behavior:
/// - Saves the "resume stage" so Login can continue from where the player reached.
/// - Uses CloudSaveStore directly (so we can save the target stage even though GameFlowManager.CurrentStage has a private setter).
/// </summary>
public class SceneDoorInteractable : Interactable
{
    [Header("Scene")]
    [SerializeField] private string targetSceneName = "Level_2";

    [Header("Auto Save")]
    [Tooltip("If enabled, saves progress right before loading the target scene.")]
    [SerializeField] private bool autoSaveBeforeLoad = true;

    [Tooltip("Optional. If set (>0), this is the stage number to save for Continue.")]
    [SerializeField] private int targetStageNumber = 0;

    [Header("Lock (optional)")]
    [SerializeField] private string requiredItemId = "key";

    [TextArea(1, 3)]
    [SerializeField] private string lockedMessage = "The door is locked. You need a key.";

    [TextArea(1, 3)]
    [SerializeField] private string openMessage = "Door opened!";

    private bool _busy;

    public override async void Interact(PlayerInteraction interactor)
    {
        if (_busy) return;
        if (interactor == null) return;

        bool requires = !string.IsNullOrWhiteSpace(requiredItemId);
        if (requires && !HasItem(interactor.gameObject, requiredItemId))
        {
            interactor.ShowInfo(lockedMessage);
            return;
        }

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            interactor.ShowInfo("Door error: targetSceneName is empty.");
            return;
        }

        _busy = true;

        try
        {
            interactor.ShowInfo(openMessage);

            if (autoSaveBeforeLoad)
            {
                int stageToSave = ResolveStageToSave(targetSceneName, targetStageNumber);
                await SaveProgressAsync(interactor.gameObject, stageToSave);
            }
        }
        catch (Exception e)
        {
            // If save fails (offline / not logged in / UGS error), still allow scene transition.
            Debug.LogWarning($"Auto-save failed, continuing anyway: {e.Message}");
            Debug.LogException(e);
        }
        finally
        {
            _busy = false;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private static bool HasItem(GameObject player, string itemId)
    {
        if (player == null) return false;

        PlayerInventory inv = player.GetComponent<PlayerInventory>();
        if (inv == null) return false;

        return inv.HasItem(itemId);
    }

    private static int ResolveStageToSave(string sceneName, int targetStageOverride)
    {
        if (targetStageOverride > 0)
            return targetStageOverride;

        // Try to parse a number from the scene name, e.g. "Level_2" -> 2, "Stage3" -> 3.
        int parsed = TryParseTrailingNumber(sceneName);
        if (parsed > 0) return parsed;

        // Fallback: save current stage (Continue will start from current stage if parsing fails).
        if (GameFlowManager.Instance != null)
            return Mathf.Max(1, GameFlowManager.Instance.CurrentStage);

        return 1;
    }

    private static int TryParseTrailingNumber(string s)
    {
        if (string.IsNullOrWhiteSpace(s)) return 0;

        // Find the last run of digits anywhere (common: Level_2, Level2, Stage_03).
        Match m = Regex.Match(s, @"(\d+)(?!.*\d)");
        if (!m.Success) return 0;

        return int.TryParse(m.Groups[1].Value, out int n) ? n : 0;
    }

    private static async System.Threading.Tasks.Task SaveProgressAsync(GameObject player, int stageToSave)
    {
        if (CloudSaveStore.Instance == null)
            throw new InvalidOperationException("CloudSaveStore.Instance is missing (is the UGS bootstrap in the scene?).");

        PlayerInventory inv = player != null ? player.GetComponent<PlayerInventory>() : null;

        // Convert PlayerInventory.SimpleItemStack -> CloudSaveStore.ItemStack
        var stacks = new List<CloudSaveStore.ItemStack>();

        if (inv != null)
        {
            var simple = inv.GetItemsForSave();
            if (simple != null)
            {
                foreach (var s in simple)
                {
                    if (s == null) continue;
                    if (string.IsNullOrWhiteSpace(s.itemId)) continue;
                    if (s.amount <= 0) continue;

                    stacks.Add(new CloudSaveStore.ItemStack { itemId = s.itemId, amount = s.amount });
                }
            }
        }

        await CloudSaveStore.Instance.SaveAsync(Mathf.Max(1, stageToSave), stacks);
    }
}
