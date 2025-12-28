using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ZombieChasePlayer2DTests
{
    private T GetPrivateField<T>(object instance, string fieldName)
    {
        var field = instance.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found.");
        return (T)field.GetValue(instance);
    }

    private void SetPrivateField(object instance, string fieldName, object value)
    {
        var field = instance.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found.");
        field.SetValue(instance, value);
    }

    [Test]
    public void DefaultValues_AreReasonable()
    {
        var zombieObject = new GameObject("Zombie");
        zombieObject.AddComponent<Rigidbody2D>();
        var zombie = zombieObject.AddComponent<ZombieChasePlayer2D>();

        float moveSpeed    = GetPrivateField<float>(zombie, "moveSpeed");
        float detectionRad = GetPrivateField<float>(zombie, "detectionRadius");
        float loseRad      = GetPrivateField<float>(zombie, "loseRadius");

        Assert.Greater(moveSpeed, 0f, "moveSpeed should be positive.");
        Assert.Greater(loseRad, detectionRad,
            "loseRadius should be larger than detectionRadius.");

        Object.DestroyImmediate(zombieObject);
    }

   [Test]
public void Awake_MatchesPlayerSpeed_WhenPlayerExists()
{
    // Create player first so the zombie can find it
    var playerObject = new GameObject("Player");
    playerObject.AddComponent<Rigidbody2D>();
    var playerMovement = playerObject.AddComponent<PlayerMovement>();

    // Set player's normalSpeed via reflection
    SetPrivateField(playerMovement, "normalSpeed", 6f);

    // Create zombie afterwards
    var zombieObject = new GameObject("Zombie");
    zombieObject.AddComponent<Rigidbody2D>();
    var zombie = zombieObject.AddComponent<ZombieChasePlayer2D>();

    // Explicitly call Awake on the zombie to ensure the logic runs
    var awakeMethod = typeof(ZombieChasePlayer2D)
        .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
    Assert.IsNotNull(awakeMethod, "Awake method not found on ZombieChasePlayer2D.");
    awakeMethod.Invoke(zombie, null);

    // Read moveSpeed after Awake
    float zombieSpeed = GetPrivateField<float>(zombie, "moveSpeed");

    Assert.AreEqual(6f, zombieSpeed, 0.0001f,
        "Zombie moveSpeed should match player's normalSpeed when matchPlayerSpeed is true.");

    Object.DestroyImmediate(playerObject);
    Object.DestroyImmediate(zombieObject);
}

}
