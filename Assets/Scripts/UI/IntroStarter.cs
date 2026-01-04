using UnityEngine;

public class IntroStarter : MonoBehaviour
{
    private void Start()
    {
        UIManager.Instance.ShowNotice(
            "Intro",
            "You wake up on a cold hospital floor...\n\nExplore. Stay quiet. Find the exit before the hospital finds you.",
            "New Start"
        );
    }
}
