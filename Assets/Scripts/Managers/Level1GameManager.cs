using UnityEngine;

/// <summary>
/// Keeps simple level state for Level 1: whether the player has the keycard,
/// and whether the player picked up the photo.
/// </summary>
public class Level1GameManager : MonoBehaviour
{
    public static Level1GameManager Instance { get; private set; }

    public bool HasKeycard { get; private set; }
    public bool HasPhoto { get; private set; }   // NEW

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void GiveKeycard()
    {
        HasKeycard = true;
        Debug.Log("Player obtained keycard.");
    }

    public void GivePhoto()                      // NEW
    {
        HasPhoto = true;
        Debug.Log("Player picked up the photo.");
    }
}
