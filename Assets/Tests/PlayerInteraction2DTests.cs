using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class PlayerInteraction2DTests
{
    private GameObject playerObject;
    private PlayerInteraction2D interaction;
    private Collider2D playerCollider;

    private GameObject otherObject;
    private Collider2D otherCollider;
    private TestInteractable2D interactable;

    /// <summary>
    /// Simple test double for Interactable2D used in these tests.
    /// </summary>
    private class TestInteractable2D : Interactable2D
    {
        public bool interacted = false;

        public override void Interact(PlayerInteraction2D interactor)
        {
            interacted = true;
        }
    }

    [SetUp]
    public void Setup()
    {
        // Create player object with a trigger collider and the interaction script
        playerObject = new GameObject("Player");
        playerCollider = playerObject.AddComponent<BoxCollider2D>();
        playerCollider.isTrigger = true;

        interaction = playerObject.AddComponent<PlayerInteraction2D>();
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
    private Interactable2D GetCurrentTarget()
    {
        var field = typeof(PlayerInteraction2D)
            .GetField("currentTarget", BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(field, "Field 'currentTarget' was not found via reflection.");
        return (Interactable2D)field.GetValue(interaction);
    }

  
    /// Helper to invoke a private method (OnTriggerEnter2D / OnTriggerExit2D).
    private void CallPrivateMethod(string methodName, params object[] args)
    {
        var method = typeof(PlayerInteraction2D)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, $"Method '{methodName}' was not found via reflection.");
        method.Invoke(interaction, args);
    }

    [Test]
    public void OnTriggerEnter2D_WithNonInteractable_DoesNotSetCurrentTarget()
    {
        //other object with collider but no Interactable2D
        otherObject = new GameObject("Other");
        otherCollider = otherObject.AddComponent<BoxCollider2D>();

        //simulate trigger enter
        CallPrivateMethod("OnTriggerEnter2D", otherCollider);

        //currentTarget should remain null
        Assert.IsNull(GetCurrentTarget(),
            "currentTarget should remain null when collider has no Interactable2D.");
    }

    [Test]
    public void OnTriggerEnter2D_WithInteractable_SetsCurrentTarget()
    {
        //other object with collider and TestInteractable2D
        otherObject = new GameObject("InteractableObject");
        otherCollider = otherObject.AddComponent<BoxCollider2D>();
        interactable = otherObject.AddComponent<TestInteractable2D>();

        //simulate trigger enter
        CallPrivateMethod("OnTriggerEnter2D", otherCollider);

        //currentTarget should be this interactable
        Assert.AreEqual(interactable, GetCurrentTarget(),
            "currentTarget should be set to the Interactable2D on trigger enter.");
    }

    [Test]
    public void OnTriggerExit2D_WithSameInteractable_ClearsCurrentTarget()
    {
        //enter first to set currentTarget
        otherObject = new GameObject("InteractableObject");
        otherCollider = otherObject.AddComponent<BoxCollider2D>();
        interactable = otherObject.AddComponent<TestInteractable2D>();

        CallPrivateMethod("OnTriggerEnter2D", otherCollider);
        Assert.AreEqual(interactable, GetCurrentTarget(),
            "Precondition failed: currentTarget should be set after OnTriggerEnter2D.");

        //simulate exit with the same interactable
        CallPrivateMethod("OnTriggerExit2D", otherCollider);

        //currentTarget should be cleared
        Assert.IsNull(GetCurrentTarget(),
            "currentTarget should be cleared when exiting the trigger of the same interactable.");
    }

    [Test]
    public void OnTriggerExit2D_WithDifferentInteractable_DoesNotClearCurrentTarget()
    {
        //first interactable
        var firstObject = new GameObject("FirstInteractable");
        var firstCollider = firstObject.AddComponent<BoxCollider2D>();
        var firstInteractable = firstObject.AddComponent<TestInteractable2D>();

        //Second interactable (the one we will exit from)
        var secondObject = new GameObject("SecondInteractable");
        var secondCollider = secondObject.AddComponent<BoxCollider2D>();
        var secondInteractable = secondObject.AddComponent<TestInteractable2D>();

        //Set otherObject so TearDown cleans up last created object
        otherObject = secondObject;

        //Enter first interactable - becomes currentTarget
        CallPrivateMethod("OnTriggerEnter2D", firstCollider);
        Assert.AreEqual(firstInteractable, GetCurrentTarget(),
            "Precondition failed: currentTarget should be firstInteractable.");

        //exit second interactable (different from currentTarget)
        CallPrivateMethod("OnTriggerExit2D", secondCollider);

        //currentTarget should remain the first interactable
        Assert.AreEqual(firstInteractable, GetCurrentTarget(),
            "currentTarget should not be cleared when exiting a different interactable.");
    }
}
