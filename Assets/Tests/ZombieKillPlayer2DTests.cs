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


 [Test]
    public void ZombieTrigger_WithPlayerTag_CallsPlayerDeath()
    {
        // --- Create Player ---
        var player = new GameObject("Player");
        player.tag = "Player"; // must match ZombieKillPlayer2D.playerTag default

        // Player needs a Collider2D because ZombieKillPlayer2D receives Collider2D
        var playerCollider = player.AddComponent<BoxCollider2D>();

        // Script we expect to be disabled when the player dies
        var movement = player.AddComponent<PlayerMovement>();
        Assert.IsTrue(movement.enabled, "Precondition: movement should start enabled.");

        // Add PlayerOneHitDeath (the thing ZombieKillPlayer2D looks for)
        var playerDeath = player.AddComponent<PlayerOneHitDeath>();

        
        SetPrivateField(playerDeath, "disableOnDeath", new MonoBehaviour[] { movement });

        // --- Create Zombie ---
        var zombie = new GameObject("Zombie");
        zombie.AddComponent<Rigidbody2D>(); // not strictly needed for this reflection call, but ok
        zombie.AddComponent<BoxCollider2D>();

        var zombieKill = zombie.AddComponent<ZombieKillPlayer2D>();

        // Call Awake so disableRootOnKill gets initialized (safe)
        InvokePrivateMethod(zombieKill, "Awake");

        
        InvokePrivateMethod(zombieKill, "OnTriggerEnter2D", playerCollider);

        // --- Assert: player "died" -> movement disabled ---
        Assert.IsFalse(movement.enabled,
            "Expected zombie to kill the player (PlayerOneHitDeath.Kill) which disables scripts in disableOnDeath.");

        Object.DestroyImmediate(zombie);
        Object.DestroyImmediate(player);
    }


    private static void InvokePrivateMethod(object target, string methodName, params object[] args)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var types = args == null ? System.Type.EmptyTypes : System.Array.ConvertAll(args, a => a.GetType());

        // Try exact signature first
        var method = target.GetType().GetMethod(methodName, flags, null, types, null);

        Assert.IsNotNull(method, $"Method '{methodName}' not found on {target.GetType().Name}.");
        method.Invoke(target, args);
    }

    private static void SetPrivateField(object target, string fieldName, object value)
    {
        var flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;
        var field = target.GetType().GetField(fieldName, flags);

        Assert.IsNotNull(field, $"Field '{fieldName}' not found on {target.GetType().Name}.");
        field.SetValue(target, value);
    }



}
