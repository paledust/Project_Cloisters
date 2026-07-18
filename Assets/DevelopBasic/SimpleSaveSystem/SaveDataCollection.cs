using System;
using UnityEngine;

[Serializable]
public class SettingSaveData
{
    public int localeIndex;
}

public class GlobalSaveData{}

[Serializable]
public class AudioSettingData
{
    public float masterVolume;
    public float ambVolume;
    public float musVolume;
    public float sfxVolume;
}

[Serializable]
public class PlayerSaveData
{
    public int levelIndex;
    public int localeIndex;
}