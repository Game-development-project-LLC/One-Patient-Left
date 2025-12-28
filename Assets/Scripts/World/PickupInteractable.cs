using System;
using System.Reflection;
using UnityEngine;

/// <summary>
/// Press interact near the object -> add item to player's inventory.
/// Also shows a pickup message and optionally disables the object.
/// Message disappears when the player moves (handled by PlayerInteraction).
/// </summary>
public class PickupInteractable : Interactable
{
    [Header("Pickup")]
    [SerializeField] private string itemId = "key";
    [Min(1)]
    [SerializeField] private int amount = 1;

    [Header("UI")]
    [SerializeField] private string pickupMessageTemplate = "Picked up: {item}";
    [SerializeField] private bool disableOnPickup = true;

    public override void Interact(PlayerInteraction interactor)
    {
        if (interactor == null) return;

        if (string.IsNullOrWhiteSpace(itemId))
        {
            interactor.ShowInfo("Pickup error: itemId is empty.");
            return;
        }

        bool added = TryAddItem(interactor.gameObject, itemId, amount);

        if (!added)
        {
            interactor.ShowInfo($"No inventory receiver found for item '{itemId}'.");
            return;
        }

        string msg = (pickupMessageTemplate ?? "Picked up: {item}").Replace("{item}", itemId);
        interactor.ShowInfo(msg);

        if (disableOnPickup)
        {
            gameObject.SetActive(false);
        }
    }

    private static bool TryAddItem(GameObject player, string id, int count)
    {
        if (player == null) return false;

        // Common method names people use in PlayerInventory.
        string[] methodNames =
        {
            "AddItem", "Add", "AddToInventory", "GiveItem", "Pickup"
        };

        var behaviours = player.GetComponents<MonoBehaviour>();
        foreach (var b in behaviours)
        {
            if (b == null) continue;
            Type t = b.GetType();

            foreach (string name in methodNames)
            {
                // Try (string, int)
                MethodInfo m2 = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, new[] { typeof(string), typeof(int) }, null);
                if (m2 != null)
                {
                    m2.Invoke(b, new object[] { id, count });
                    return true;
                }

                // Try (string)
                MethodInfo m1 = t.GetMethod(name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    null, new[] { typeof(string) }, null);
                if (m1 != null)
                {
                    m1.Invoke(b, new object[] { id });
                    return true;
                }
            }
        }

        return false;
    }
}
