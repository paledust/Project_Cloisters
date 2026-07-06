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
        public static void SetAudioVolume(AudioSettingType audioSettingType, float volume)
        {
            if(AudioManager.Instance==null)
                return;
            switch(audioSettingType)
            {
                case AudioSettingType.Master:
                    AudioManager.Instance.ChangeMasterVolume(volume);
                    break;
                case AudioSettingType.AMB:
                    AudioManager.Instance.ChangeAMBVolume(volume);
                    break;
                case AudioSettingType.MUS:
                    AudioManager.Instance.ChangeMUSVolume(volume);
                    break;
                case AudioSettingType.SFX:
                    AudioManager.Instance.ChangeSFXVolume(volume);
                    break;
            }
        }
    }
}
