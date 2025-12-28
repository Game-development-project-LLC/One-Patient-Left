using System;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Interactable door:
/// - If requiredItemId is set: player must have it.
/// - If allowed: loads target scene (by name).
/// </summary>
public class SceneDoorInteractable : Interactable
{
    [Header("Scene")]
    [SerializeField] private string targetSceneName = "Level_2";

    [Header("Lock (optional)")]
    [SerializeField] private string requiredItemId = "key";
    [TextArea(1, 3)]
    [SerializeField] private string lockedMessage = "The door is locked. You need a key.";
    [TextArea(1, 3)]
    [SerializeField] private string openMessage = "Door opened!";

    public override void Interact(PlayerInteraction interactor)
    {
        if (interactor == null) return;

        bool requires = !string.IsNullOrWhiteSpace(requiredItemId);
        if (requires && !TryHasItem(interactor.gameObject, requiredItemId))
        {
            interactor.ShowInfo(lockedMessage);
            return;
        }

        interactor.ShowInfo(openMessage);

        if (string.IsNullOrWhiteSpace(targetSceneName))
        {
            interactor.ShowInfo("Door error: targetSceneName is empty.");
            return;
        }

        SceneManager.LoadScene(targetSceneName);
    }

    private static bool TryHasItem(GameObject player, string id)
    {
        if (player == null) return false;

        string[] methodNames =
        {
            "HasItem", "ContainsItem", "Contains", "Has"
        };

        var behaviours = player.GetComponents<MonoBehaviour>();
        foreach (var b in behaviours)
        {
            if (b == null) continue;
            Type t = b.GetType();

            foreach (string name in methodNames)
            {
                MethodInfo m = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, new[] { typeof(string) }, null);
                if (m != null && m.ReturnType == typeof(bool))
                {
                    object result = m.Invoke(b, new object[] { id });
                    if (result is bool ok) return ok;
                }
            }
        }

        // If no inventory script found, treat as "no item".
        return false;
    }
}
