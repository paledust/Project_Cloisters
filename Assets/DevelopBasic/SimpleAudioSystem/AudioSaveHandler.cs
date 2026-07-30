using UnityEngine;
using SimpleSaveSystem;
using System;

namespace SimpleAudioSystem
{
    public class AudioSaveHandler : MonoBehaviour, ISaveable
    {
        [SerializeField, ShowOnly] private string byteGuid = Guid.NewGuid().ToString();
        
        public Guid guid => new Guid(byteGuid);

        public void RestoreState(PlayerSaveData state)
        {
            var audioSetting = state.audioSettingData;
            AudioSettingService.SetAudioVolume(AudioSettingType.Master, audioSetting.masterVolume);
            AudioSettingService.SetAudioVolume(AudioSettingType.AMB, audioSetting.ambVolume);
            AudioSettingService.SetAudioVolume(AudioSettingType.MUS, Mathf.Min(0.5f, audioSetting.musVolume));
            AudioSettingService.SetAudioVolume(AudioSettingType.SFX, audioSetting.sfxVolume);
        }
        public void CaptureState(ref PlayerSaveData saveData)
        {
            if(saveData.audioSettingData==null)
                saveData.audioSettingData = AudioSettingData.defaultSetting;

            saveData.audioSettingData.masterVolume = AudioSettingService.GetAudioVolume(AudioSettingType.Master);
            saveData.audioSettingData.ambVolume = AudioSettingService.GetAudioVolume(AudioSettingType.AMB);
            saveData.audioSettingData.musVolume = AudioSettingService.GetAudioVolume(AudioSettingType.MUS);
            saveData.audioSettingData.sfxVolume = AudioSettingService.GetAudioVolume(AudioSettingType.SFX);
        }
    }
}
