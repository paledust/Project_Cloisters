using SimpleAudioSystem;
using UnityEngine;

public class AmbienceHandler : MonoBehaviour
{
    private AudioManager audioManager;
    public void Init(AudioManager audioManager)
    {
        this.audioManager = audioManager;
    }
    public void CleanUp()
    {
        FadeOutAmbience(0.5f, true);
    }
    public void FadeOutAmbience(float duration, bool stopAfterFade = false)
    {
        audioManager.FadeAmbience(0, duration, stopAfterFade);
    }
    public void PlayAmbience(string ambienceName, float volume)
    {
        audioManager.PlayAmbience(ambienceName, true, volume, false);
    }
}
