using NUnit.Framework;
using UnityEngine;
using System.Reflection;

public class PlayerInventoryTests
{
   private GameObject playerObject;
    private PlayerInventory inventory;

    [SetUp]
    public void Setup()
    {
        // Create a clean Player with an inventory component
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
        
        var itemPhoto = ScriptableObject.CreateInstance<ItemDefinition>();

        
        SetField(itemPhoto, "id", "key2");
        SetField(itemPhoto, "displayName", "photo2");

       
        inventory.AddItem(itemPhoto);

      
        Assert.IsTrue(inventory.HasItem(itemPhoto),
            "Expected item to be added to inventory.");
    }


    [Test]
    public void AddItem_DuplicateItem_IsNotAddedTwice()
    {

        var itemPhoto2 =  ScriptableObject.CreateInstance<ItemDefinition>();
        
        
        SetField(itemPhoto2, "id", "key2");
        SetField(itemPhoto2, "displayName", "photo2");

        inventory.AddItem(itemPhoto2);
        inventory.AddItem(itemPhoto2);
        
        string inventoryText = inventory.GetInventoryText();

        int firstIndex = inventoryText.IndexOf("key2");
        int lastIndex = inventoryText.LastIndexOf("key2");

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
        var itemPhoto3 = ScriptableObject.CreateInstance<ItemDefinition>();
        var itemTable = ScriptableObject.CreateInstance<ItemDefinition>();

        SetField(itemPhoto3, "id", "key4");
        SetField(itemPhoto3, "displayName", "photo4");

        inventory.AddItem(itemPhoto3);

        Assert.IsTrue(inventory.HasItem(itemPhoto3));
        Assert.IsFalse(inventory.HasItem(itemTable));
    }

    [Test]
    public void GetInventoryText_EmptyInventory_ReturnsEmptyText()
    {
        string text = inventory.GetInventoryText();

        Assert.AreEqual("Inventory: (empty)", text,
            "Empty inventory should return the correct empty message.");
    }

    private static void SetField(ItemDefinition target, string fieldName, string value)
        {
            var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
            var field = target.GetType().GetField(fieldName, flags);

            Assert.IsNotNull(field,
                $"Field '{fieldName}' was not found on {target.GetType().Name}. " +
                $"Open ItemDefinition.cs and check the exact private field name (it might be '_id' or 'itemId').");

            field.SetValue(target, value);
        }
}
