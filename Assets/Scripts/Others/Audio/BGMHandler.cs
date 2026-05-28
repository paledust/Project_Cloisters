using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class BGMHandler : MonoBehaviour
{
    private AudioManager audioManager;
    public void Init(AudioManager audioManager)
    {
        this.audioManager = audioManager;
    }
    public void CleanUp()
    {
        FadeOutMusic(0.5f, true);
    }
    public void FadeOutMusic(float duration, bool stopAfterFade)
    {
        audioManager.FadeMusic(0, duration, stopAfterFade);
    }
    public void FadeOutMusic(float duration) => FadeOutMusic(duration, false);
    public void PlayMusic(string bgmName, float volume, float transition = 0.5f)
    {
        audioManager.PlayMusic(bgmName, true, transition, volume);
    }
    public async void PlayMusicIntroToLoop(string introName, string loopName, float volume, float transition)
    {
        audioManager.PlayMusic(introName, true, transition, volume);
        audioManager.PlayMusic(loopName, true, 60f, volume);
    }
}
