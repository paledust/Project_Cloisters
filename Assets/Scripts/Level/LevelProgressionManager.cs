using UnityEngine;
using SimpleSaveSystem;
using System;

public class LevelProgressionManager : Singleton<LevelProgressionManager>, ISaveable
{
    [SerializeField, ShowOnly] private string byteGuid = Guid.NewGuid().ToString();
    private int levelProgress = 0;
    [SerializeField, ShowOnly] private bool isFreshStart = true;

    public int LevelProgress => levelProgress;
    public bool IsFreshStart => isFreshStart;
    public Guid guid => new Guid(byteGuid);

    public void SetProgress(int progress)
    {
        levelProgress = progress;
        isFreshStart = false;
    }
    public void ResetProgress() => levelProgress = 0;
    public void RestoreState(PlayerSaveData state)
    {
        levelProgress = state.levelIndex;
        isFreshStart = state.isNew;
        EventHandler.Call_OnLevelStateRestored();
    }
    public void CaptureState(ref PlayerSaveData saveData)
    {
        saveData.levelIndex = levelProgress;
        saveData.isNew = isFreshStart;
    }
}
