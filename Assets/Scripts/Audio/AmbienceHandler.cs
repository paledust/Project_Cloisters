using SimpleAudioSystem;
using UnityEngine;

public class AmbienceHandler : MonoBehaviour
{
    public void Init(){}
    public void CleanUp()
    {
        FadeOutAmbience(0.5f, true);
    }
    public void FadeOutAmbience(float duration, bool stopAfterFade = false)
    {
        AudioManager.Instance.FadeAmbience(0, duration, stopAfterFade);
    }
    public void PlayAmbience(string ambienceName, float volume)
    {
        AudioManager.Instance.PlayAmbience(ambienceName, true, volume, false);
    }
    public void PlayAmbience(string ambienceName, float transitionTime, float volume)
    {
        AudioManager.Instance.PlayAmbience(ambienceName, true, transitionTime, volume);
    }
}
