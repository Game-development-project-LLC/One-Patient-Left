using System.Reflection;
using NUnit.Framework;
using UnityEngine;

public class PlayerMovement2DTests
{
    private GameObject playerObject;
    private PlayerMovement player;
    private Rigidbody2D rb;

    [SetUp]
    public void Setup()
    {
        playerObject = new GameObject("Player");
        rb = playerObject.AddComponent<Rigidbody2D>();
        player = playerObject.AddComponent<PlayerMovement>();
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
    }

    [Test]
    public void PlayerObject_HasRequiredComponents()
    {
        Assert.IsNotNull(player, "PlayerMovement2D component should exist");
        Assert.IsNotNull(rb, "Rigidbody2D component should exist");
    }
    [Test]
    public void Player_DefaultValues_AreValid()
    {
        Assert.Greater(player.getNormalSpeed(), 0f, "normalSpeed should be > 0");
        Assert.Greater(player.getSlowSpeed(), 0f, "slowSpeed should be > 0");
        Assert.GreaterOrEqual(player.getNormalSpeed(), player.getSlowSpeed(),
            "normalSpeed is usually expected to be >= slowSpeed");
        Assert.IsFalse(player.IsUsingSlowSpeed, "Player should not start in slow mode");
    }

 
}
