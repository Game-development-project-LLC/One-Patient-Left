using NUnit.Framework;
using UnityEngine;

public class PlayerInventoryTests
{
    private GameObject playerObject;
    private PlayerInventory inventory;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("Player");
        inventory = playerObject.AddComponent<PlayerInventory>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
    }

    [Test]
    public void AddItem_ValidItem_IsAddedToInventory()
    {
        inventory.AddItem("photo");

        Assert.IsTrue(inventory.HasItem("photo"),
            "Expected item to be added to inventory.");
    }

    [Test]
    public void AddItem_DuplicateItem_IsNotAddedTwice()
    {
        inventory.AddItem("key");
        inventory.AddItem("key");

        string inventoryText = inventory.GetInventoryText();

        int firstIndex = inventoryText.IndexOf("key");
        int lastIndex = inventoryText.LastIndexOf("key");

        Assert.AreEqual(firstIndex, lastIndex,
            "Duplicate item should not be added twice.");
    }

    [Test]
    public void AddItem_EmptyOrNull_IsIgnored()
    {
        inventory.AddItem("");
        inventory.AddItem("   ");

        Assert.IsFalse(inventory.HasItem(""),
            "Empty item should not be added.");
        Assert.AreEqual("Inventory: (empty)", inventory.GetInventoryText(),
            "Inventory should remain empty when adding invalid items.");
    }

    [Test]
    public void HasItem_ReturnsTrueOnlyForExistingItems()
    {
        inventory.AddItem("staff_keycard");

        Assert.IsTrue(inventory.HasItem("staff_keycard"));
        Assert.IsFalse(inventory.HasItem("photo"));
    }

    [Test]
    public void GetInventoryText_EmptyInventory_ReturnsEmptyText()
    {
        string text = inventory.GetInventoryText();

        Assert.AreEqual("Inventory: (empty)", text,
            "Empty inventory should return the correct empty message.");
    }

    [Test]
    public void GetInventoryText_WithItems_ReturnsFormattedList()
    {
        inventory.AddItem("photo");
        inventory.AddItem("map");

        string text = inventory.GetInventoryText();

        Assert.IsTrue(text.Contains("photo"));
        Assert.IsTrue(text.Contains("map"));
        Assert.IsTrue(text.StartsWith("Inventory:"),
            "Inventory text should start with 'Inventory:'");
    }
}
