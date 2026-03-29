using UnityEngine;

public class LevelProgressionManager : Singleton<LevelProgressionManager>
{
    private int levelProgress = 0;
    public int LevelProgress => levelProgress;

    public void SetLevelProgress(int newProgress)
    {
        levelProgress = Mathf.Max(newProgress, 0);
    }
    public void ResetProgression()
    {
        levelProgress = 0;
    }
}
