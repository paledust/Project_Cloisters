using SimpleAudioSystem;
using UnityEngine;

public class BGMHandler : MonoBehaviour
{
    public bool isInitialized{get; private set;} = false;
    protected AudioManager audioManager => AudioManager.Instance;
    public void Init()
    {
        if(isInitialized)
            return;
        isInitialized = true;
        OnInit();
    }
    public void CleanUp() => FadeOutMusic(0.5f, true);
    public void FadeOutMusic(float duration, bool stopAfterFade) => audioManager.FadeMusic(0, duration, stopAfterFade);
    public void FadeOutMusic(float duration) => FadeOutMusic(duration, false);
    public virtual void PlayMusic(string bgmName, float volume, float transition = 0.5f)=>audioManager.PlayMusic(bgmName, true, transition, volume, false);
    protected virtual void OnInit() {}
}
