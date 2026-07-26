using UnityEngine;
using SimpleSaveSystem;
using System;

public class LevelProgressionManager : Singleton<LevelProgressionManager>, ISaveable
{
    [SerializeField, ShowOnly] private string byteGuid = Guid.NewGuid().ToString();
    private int levelProgress = 0;

    public int LevelProgress => levelProgress;
    public Guid guid => new Guid(byteGuid);

    public void SetProgress(int progress) => levelProgress = progress;
    public void ResetProgress() => levelProgress = 0;
    public void RestoreState(PlayerSaveData state)
    {
        levelProgress = state.levelIndex;
    }
    public void CaptureState(ref PlayerSaveData saveData)
    {
        saveData.levelIndex = levelProgress;
    }
}
