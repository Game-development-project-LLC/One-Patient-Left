//using System.Reflection;
//using NUnit.Framework;
//using UnityEngine;

//public class SimpleInteractableTests
//{
//    private GameObject interactableObject;
//    private SimpleInteractable interactable;

//    private GameObject playerObject;
//    private PlayerInteraction playerInteraction;
//    private PlayerInventory playerInventory;

//    [SetUp]
//    public void Setup()
//    {
//        // Interactable
//        interactableObject = new GameObject("SimpleInteractable");
//        interactable = interactableObject.AddComponent<SimpleInteractable>();

//        // Player with required components
//        playerObject = new GameObject("Player");
//        playerObject.AddComponent<BoxCollider>(); // required by PlayerInteraction
//        playerInteraction = playerObject.AddComponent<PlayerInteraction>();
//        playerInventory = playerObject.AddComponent<PlayerInventory>();
//    }

//    [TearDown]
//    public void TearDown()
//    {
//        Object.DestroyImmediate(interactableObject);
//        Object.DestroyImmediate(playerObject);
//    }

//    // Helper to set a private serialized field
//    private void SetPrivateField(string fieldName, object value)
//    {
//        var field = typeof(SimpleInteractable)
//            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
//        Assert.IsNotNull(field, $"Field '{fieldName}' not found.");
//        field.SetValue(interactable, value);
//    }

//    [Test]
//    public void Interact_PickupItem_AddsItemToInventory()
//    {
//        SetPrivateField("interactionType", SimpleInteractable.SimpleInteractionType.PickupItem);
//        SetPrivateField("itemId", "photo");
//        SetPrivateField("canInteractMultipleTimes", true);

//        Assert.IsFalse(playerInventory.HasItem("photo"));

//        interactable.Interact(playerInteraction);

//        Assert.IsTrue(playerInventory.HasItem("photo"),
//            "PickupItem should add the item to the player's inventory.");
//    }

//    [Test]
//    public void Interact_WhenAlreadyInteractedAndNotMultiple_DoesNothing()
//    {
//        SetPrivateField("interactionType", SimpleInteractable.SimpleInteractionType.PickupItem);
//        SetPrivateField("itemId", "photo");
//        SetPrivateField("canInteractMultipleTimes", false);
//        SetPrivateField("alreadyInteracted", true);

//        interactable.Interact(playerInteraction);

//        Assert.IsFalse(playerInventory.HasItem("photo"),
//            "When alreadyInteracted is true and canInteractMultipleTimes is false, no item should be added.");
//    }

//    [Test]
//    public void Interact_RemoveObject_DeactivatesTarget()
//    {
//        var targetObject = new GameObject("TargetToRemove");

//        SetPrivateField("interactionType", SimpleInteractable.SimpleInteractionType.RemoveObject);
//        SetPrivateField("objectToRemove", targetObject);
//        SetPrivateField("deactivateInsteadOfDestroy", true);

//        Assert.IsTrue(targetObject.activeSelf);

//        interactable.Interact(playerInteraction);

//        Assert.IsFalse(targetObject.activeSelf,
//            "RemoveObject should deactivate the target when deactivateInsteadOfDestroy is true.");

//        Object.DestroyImmediate(targetObject);
//    }
//}
