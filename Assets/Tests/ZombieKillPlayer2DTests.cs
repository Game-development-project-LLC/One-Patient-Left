using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class ZombieKillPlayer2DTests
{
    [Test]
    public void Awake_EnsuresColliderIsNotTrigger()
    {
        //create zombie object with Rigidbody2D and Collider2D
        var zombieObject = new GameObject("Zombie");
        zombieObject.AddComponent<Rigidbody2D>();
        var collider = zombieObject.AddComponent<BoxCollider2D>();

        // Set collider to trigger before running Awake
        collider.isTrigger = true;

        
        var zombie = zombieObject.AddComponent<ZombieKillPlayer2D>();

        // Explicitly call Awake via reflection
        var awakeMethod = typeof(ZombieKillPlayer2D)
            .GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
        Assert.IsNotNull(awakeMethod, "Awake method not found on ZombieKillPlayer2D.");
        awakeMethod.Invoke(zombie, null);

        // Assert: collider should now be non-trigger
        Assert.IsFalse(collider.isTrigger,
            "ZombieKillPlayer2D should set its Collider2D to non-trigger in Awake.");

        Object.DestroyImmediate(zombieObject);
    }
}
