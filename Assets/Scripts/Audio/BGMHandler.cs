using SimpleAudioSystem;
using UnityEngine;

public class BGMHandler : MonoBehaviour
{
    public bool isInitialized{get; private set;} = false;
    protected AudioManager audioManager;
    public void Init(AudioManager audioManager)
    {
        if(isInitialized)
            return;
        isInitialized = true;
        this.audioManager = audioManager;
    }
    public void CleanUp() => FadeOutMusic(0.5f, true);
    public void FadeOutMusic(float duration, bool stopAfterFade) => audioManager.FadeMusic(0, duration, stopAfterFade);
    public void FadeOutMusic(float duration) => FadeOutMusic(duration, false);
    public virtual void PlayMusic(string bgmName, float volume, float transition = 0.5f)=>audioManager.PlayMusic(bgmName, true, transition, volume, false, true);
}
