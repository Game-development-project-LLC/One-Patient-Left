using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class PlayerInteractionTests
{
    private GameObject playerObject;
    private PlayerInteraction interaction;
    private Collider playerCollider;

    private GameObject otherObject;
    private Collider otherCollider;
    private TestInteractable interactable;

    /// <summary>
    /// Simple test double for Interactable used in these tests.
    /// </summary>
    private class TestInteractable : Interactable
    {
        public bool interacted = false;

        public override void Interact(PlayerInteraction interactor)
        {
            interacted = true;
        }
    }

    [SetUp]
    public void Setup()
    {
        // Create player object with a trigger collider and the interaction script
        playerObject = new GameObject("Player");
        playerCollider = playerObject.AddComponent<BoxCollider>();
        playerCollider.isTrigger = true;

        interaction = playerObject.AddComponent<PlayerInteraction>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        if (otherObject != null)
        {
            Object.DestroyImmediate(otherObject);
        }
    }

 
    /// Helper to get the private 'currentTarget' field via reflection.
    private Interactable GetCurrentTarget()
    {
        var field = typeof(PlayerInteraction)
            .GetField("currentTarget", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(field, "Field 'currentTarget' was not found via reflection.");
        return (Interactable)field.GetValue(interaction);
    }

  
    /// Helper to invoke a private method (OnTriggerEnter / OnTriggerExit).
    private void CallPrivateMethod(string methodName, params object[] args)
    {
        var method = typeof(PlayerInteraction)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, $"Method '{methodName}' was not found via reflection.");
        method.Invoke(interaction, args);
    }

    [Test]
    public void OnTriggerEnter_WithNonInteractable_DoesNotSetCurrentTarget()
    {
        //other object with collider but no Interactable
        otherObject = new GameObject("Other");
        otherCollider = otherObject.AddComponent<BoxCollider>();

        //simulate trigger enter
        CallPrivateMethod("OnTriggerEnter", otherCollider);

        //currentTarget should remain null
        Assert.IsNull(GetCurrentTarget(),
            "currentTarget should remain null when collider has no Interactable.");
    }

    [Test]
    public void OnTriggerEnter_WithInteractable_SetsCurrentTarget()
    {
        //other object with collider and TestInteractable
        otherObject = new GameObject("InteractableObject");
        otherCollider = otherObject.AddComponent<BoxCollider>();
        interactable = otherObject.AddComponent<TestInteractable>();

        //simulate trigger enter
        CallPrivateMethod("OnTriggerEnter", otherCollider);

        //currentTarget should be this interactable
        Assert.AreEqual(interactable, GetCurrentTarget(),
            "currentTarget should be set to the Interactable on trigger enter.");
    }

    [Test]
    public void OnTriggerExit_WithSameInteractable_ClearsCurrentTarget()
    {
        //enter first to set currentTarget
        otherObject = new GameObject("InteractableObject");
        otherCollider = otherObject.AddComponent<BoxCollider>();
        interactable = otherObject.AddComponent<TestInteractable>();

        CallPrivateMethod("OnTriggerEnter", otherCollider);
        Assert.AreEqual(interactable, GetCurrentTarget(),
            "Precondition failed: currentTarget should be set after OnTriggerEnter.");

        //simulate exit with the same interactable
        CallPrivateMethod("OnTriggerExit", otherCollider);

        //currentTarget should be cleared
        Assert.IsNull(GetCurrentTarget(),
            "currentTarget should be cleared when exiting the trigger of the same interactable.");
    }

    [Test]
    public void OnTriggerExit_WithDifferentInteractable_DoesNotClearCurrentTarget()
    {
        //first interactable
        var firstObject = new GameObject("FirstInteractable");
        var firstCollider = firstObject.AddComponent<BoxCollider>();
        var firstInteractable = firstObject.AddComponent<TestInteractable>();

        //Second interactable (the one we will exit from)
        var secondObject = new GameObject("SecondInteractable");
        var secondCollider = secondObject.AddComponent<BoxCollider>();
        var secondInteractable = secondObject.AddComponent<TestInteractable>();

        //Set otherObject so TearDown cleans up last created object
        otherObject = secondObject;

        //Enter first interactable - becomes currentTarget
        CallPrivateMethod("OnTriggerEnter", firstCollider);
        Assert.AreEqual(firstInteractable, GetCurrentTarget(),
            "Precondition failed: currentTarget should be firstInteractable.");

        //exit second interactable (different from currentTarget)
        CallPrivateMethod("OnTriggerExit", secondCollider);

        //currentTarget should remain the first interactable
        Assert.AreEqual(firstInteractable, GetCurrentTarget(),
            "currentTarget should not be cleared when exiting a different interactable.");
    }
}
