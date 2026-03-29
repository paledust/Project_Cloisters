using UnityEngine;

public class LevelProgressionManager : Singleton<LevelProgressionManager>
{
    private int levelProgress = 0;
    public int LevelProgress => levelProgress;
    public void SetProgress(int progress)
    {
        levelProgress = progress;
    }
    public void ResetProgress()
    {
        levelProgress = 0;
    }
}
