using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ZombieChasePlayer2DTests
{
    private float GetPrivateField(object instance, string fieldName)
    {
        var field = instance.GetType()
            .GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(field, $"Field '{fieldName}' not found.");
        return (float)field.GetValue(instance);
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

        float moveSpeed    = GetPrivateField(zombie, "chaseSpeed");
        float detectionRan = GetPrivateField(zombie, "detectionRange");
        float loseRan      = GetPrivateField(zombie, "loseRangeMultiplier");

        Assert.Greater(moveSpeed, 0f, "moveSpeed should be positive.");
        Assert.Greater(detectionRan, loseRan,
            "detectionRange should be larger than loseRangeMultiplier .");
        Object.DestroyImmediate(zombieObject);
    }

   [Test]
public void Awake_MatchesPlayerSpeed_WhenPlayerExists()
{
    // Create player first so the zombie can find it
    var playerObject = new GameObject("Player");
    playerObject.AddComponent<Rigidbody2D>();
    var playerMovement = playerObject.AddComponent<PlayerMovement>();

    // Set player's slowSpeed via reflection
    SetPrivateField(playerMovement, "slowSpeed", 2.2f);

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
    float zombieSpeed = GetPrivateField(zombie,"chaseSpeed");

    Assert.AreEqual(2.2f, zombieSpeed, 0.0001f,
        "Zombie moveSpeed should match player's slowSpeed when matchPlayerSpeed is true.");

    Object.DestroyImmediate(playerObject);
    Object.DestroyImmediate(zombieObject);
}

}
