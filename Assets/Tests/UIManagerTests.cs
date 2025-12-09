using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using TMPro;  

public class UIManagerTests
{
    private GameObject uiObject;
    private UIManager ui;

    private TMP_Text promptText;
    private TMP_Text infoText;
    private GameObject gameOverPanel;
    private TMP_Text gameOverText;

    [SetUp]
    public void Setup()
    {
        
        uiObject = new GameObject("UIManager");
        ui = uiObject.AddComponent<UIManager>();
        promptText = CreateTMP("PromptText");
        infoText = CreateTMP("InfoText");
        gameOverText = CreateTMP("GameOverText");

        
        gameOverPanel = new GameObject("GameOverPanel");

        
        SetPrivateField("promptText", promptText);
        SetPrivateField("infoText", infoText);
        SetPrivateField("gameOverPanel", gameOverPanel);
        SetPrivateField("gameOverText", gameOverText);

       
        InvokePrivateMethod("Start");
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(uiObject);
        Object.DestroyImmediate(promptText.gameObject);
        Object.DestroyImmediate(infoText.gameObject);
        Object.DestroyImmediate(gameOverText.gameObject);
        Object.DestroyImmediate(gameOverPanel);

        typeof(UIManager)
            .GetProperty("Instance", BindingFlags.Static | BindingFlags.Public)
            ?.SetValue(null, null);
    }

    //Helper Methods

    private TMP_Text CreateTMP(string name)
    {
        var go = new GameObject(name);
        var text = go.AddComponent<TextMeshProUGUI>();
        return text;
    }

    private void SetPrivateField(string fieldName, object value)
    {
        var field = typeof(UIManager)
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(field, $"Field '{fieldName}' not found.");
        field.SetValue(ui, value);
    }

    private void InvokePrivateMethod(string methodName)
    {
        var method = typeof(UIManager)
            .GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);

        Assert.IsNotNull(method, $"Method '{methodName}' not found.");
        method.Invoke(ui, null);
    }

    //Tests
    [Test]
    public void ShowPrompt_SetsTextAndActivatesObject()
    {
        ui.ShowPrompt("Hello");

        Assert.AreEqual("Hello", promptText.text);
        Assert.IsTrue(promptText.gameObject.activeSelf);
    }

    [Test]
    public void HidePrompt_ClearsTextAndDeactivatesObject()
    {
        ui.ShowPrompt("Hello");
        ui.HidePrompt();

        Assert.AreEqual(string.Empty, promptText.text);
        Assert.IsFalse(promptText.gameObject.activeSelf);
    }

    [Test]
    public void ShowInfo_SetsTextAndActivatesObject()
    {
        ui.ShowInfo("Info");

        Assert.AreEqual("Info", infoText.text);
        Assert.IsTrue(infoText.gameObject.activeSelf);
    }

    [Test]
    public void ClearInfo_ClearsText()
    {
        ui.ShowInfo("Info");
        ui.ClearInfo();

        Assert.AreEqual(string.Empty, infoText.text);
    }

    [Test]
    public void ShowGameOver_SetsTextAndActivatesPanel()
    {
        ui.ShowGameOver("Game Over");

        Assert.AreEqual("Game Over", gameOverText.text);
        Assert.IsTrue(gameOverPanel.activeSelf);
    }

    [Test]
    public void HideGameOver_DeactivatesPanel()
    {
        ui.ShowGameOver("Game Over");
        ui.HideGameOver();

        Assert.IsFalse(gameOverPanel.activeSelf);
    }
}
