using System.Reflection;
using NUnit.Framework;
using TMPro;
using UnityEngine;

public class ExitDoorInteractableTests
{
    private GameObject doorObject;
    private ExitDoorInteractable exitDoor;
    private Collider2D doorCollider;

    private GameObject playerObject;
    private PlayerInteraction2D playerInteraction;
    private PlayerInventory playerInventory;

    private GameObject uiObject;
    private UIManager uiManager;
    private TMP_Text infoText;

    [SetUp]
    public void Setup()
    {
        
        uiObject = new GameObject("UIManager");
        uiManager = uiObject.AddComponent<UIManager>();

        // Make sure Awake runs so that UIManager.Instance is assigned
        var awakeMethod = typeof(UIManager)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(awakeMethod, "Could not find 'Awake' on UIManager.");
        awakeMethod.Invoke(uiManager, null);

        
        var infoGO = new GameObject("InfoText");
        infoText = infoGO.AddComponent<TextMeshProUGUI>();

        // Inject infoText into UIManager via reflection
        var infoField = typeof(UIManager)
            .GetField("infoText", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(infoField, "Could not find 'infoText' field on UIManager.");
        infoField.SetValue(uiManager, infoText);

        // Call Start() to initialize UI state
        var startMethod = typeof(UIManager)
            .GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(startMethod, "Could not find 'Start' method on UIManager.");
        startMethod.Invoke(uiManager, null);

        
        doorObject = new GameObject("ExitDoor");
        doorCollider = doorObject.AddComponent<BoxCollider2D>();
        exitDoor = doorObject.AddComponent<ExitDoorInteractable>();

        
        playerObject = new GameObject("Player");
        // Required by [RequireComponent(typeof(Collider2D))] on PlayerInteraction2D
        playerObject.AddComponent<BoxCollider2D>();
        playerInteraction = playerObject.AddComponent<PlayerInteraction2D>();
        playerInventory = playerObject.AddComponent<PlayerInventory>();
    }


    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(doorObject);
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(uiObject);
        if (infoText != null)
        {
            Object.DestroyImmediate(infoText.gameObject);
        }

        // Reset UIManager.Instance so it does not persist between tests
        var instanceProp = typeof(UIManager)
            .GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);
        instanceProp?.SetValue(null, null);
    }


    [Test]
    public void Interact_WithoutRequiredItem_ShowsLockedMessage()
    {
        // Ensure inventory does NOT contain required item
        // Default requiredItemId is "staff_keycard" so we do nothing

        
        exitDoor.Interact(playerInteraction);

        //UI should show the locked message
        string text = infoText.text;
        Assert.IsTrue(text.Contains("The door is locked"),
            "Interact without required item should show the locked message.");
    }

    [Test]
    public void Interact_WithRequiredItem_ShowsOpenMessage()
    {
        //give the player the required item
        playerInventory.AddItem("staff_keycard");

        // Ensure that nextSceneName is empty so SceneManager.LoadScene is not called in tests
        var sceneNameField = typeof(ExitDoorInteractable)
            .GetField("nextSceneName", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(sceneNameField, "Could not find 'nextSceneName' field.");
        sceneNameField.SetValue(exitDoor, string.Empty);

        
        exitDoor.Interact(playerInteraction);

        //UI should show the open message
        string text = infoText.text;
        Assert.IsTrue(text.Contains("You swipe the keycard"),
            "Interact with required item should show the open message.");
    }
}
