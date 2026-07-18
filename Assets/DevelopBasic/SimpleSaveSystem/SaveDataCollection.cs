using System;
using UnityEngine;

public class GlobalSaveData{}

[Serializable]
public class AudioSettingData
{
    public float masterVolume = 1;
    public float ambVolume = 1;
    public float musVolume = 1;
    public float sfxVolume = 1;
    public static AudioSettingData defaultSetting = new AudioSettingData();
    public AudioSettingData()
    {
        masterVolume = 1;
        ambVolume = 1;
        musVolume = 1;
        sfxVolume = 1;
    }
}

[Serializable]
public class PlayerSaveData
{
    public int levelIndex;
    public int localeIndex;
    public AudioSettingData audioSettingData;
    
    public PlayerSaveData()
    {
        levelIndex = 0;
        localeIndex = 0;
        audioSettingData = AudioSettingData.defaultSetting;
    }
}