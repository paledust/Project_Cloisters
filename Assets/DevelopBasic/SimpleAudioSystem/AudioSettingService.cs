using UnityEngine;

namespace SimpleAudioSystem
{
    public enum AudioSettingType
    {
        Master,
        AMB,
        MUS,
        SFX
    }
    public static class AudioSettingService
    {
        private static float masterVolume = AudioSettingData.defaultSetting.masterVolume;
        private static float ambVolume = AudioSettingData.defaultSetting.ambVolume;
        private static float musVolume = AudioSettingData.defaultSetting.musVolume;
        private static float sfxVolume = AudioSettingData.defaultSetting.sfxVolume;
        
        public static void SetAudioVolume(AudioSettingType audioSettingType, float volume)
        {
            if(AudioManager.Instance==null)
                return;
            volume = Mathf.Clamp01(volume);
            switch(audioSettingType)
            {
                case AudioSettingType.Master:
                    AudioManager.Instance.ChangeMasterVolume(volume);
                    masterVolume = volume;
                    break;
                case AudioSettingType.AMB:
                    AudioManager.Instance.ChangeAMBVolume(volume);
                    ambVolume = volume;
                    break;
                case AudioSettingType.MUS:
                    AudioManager.Instance.ChangeMUSVolume(volume);
                    musVolume = volume;
                    break;
                case AudioSettingType.SFX:
                    AudioManager.Instance.ChangeSFXVolume(volume);
                    sfxVolume = volume;
                    break;
            }
        }
        public static float GetAudioVolume(AudioSettingType audioSettingType)
        {
            switch(audioSettingType)
            {
                case AudioSettingType.Master:
                    return masterVolume;
                case AudioSettingType.AMB:
                    return ambVolume;
                case AudioSettingType.MUS:
                    return musVolume;
                case AudioSettingType.SFX:
                    return sfxVolume;
            }
            return 1;
        }
    }
}
