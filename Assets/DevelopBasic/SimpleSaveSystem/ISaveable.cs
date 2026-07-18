namespace SimpleSaveSystem{
    public interface ISaveable
    {
        System.Guid guid{get;}
        void RestoreState(PlayerSaveData state);
        void CaptureState(ref PlayerSaveData saveData);
    }
}