using UnityEngine;

/// <summary>
/// Base component for any object that can be killed by hazards or enemies.
/// </summary>
public abstract class Killable : MonoBehaviour
{
    /// <summary>
    /// Kills this object. The killer can be null.
    /// </summary>
    public abstract void Kill(GameObject killer);
}
