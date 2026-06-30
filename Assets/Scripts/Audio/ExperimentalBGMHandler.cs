using UnityEngine;

public class ExperimentalBGMHandler : BGMHandler
{
    [SerializeField] private float bgmLoopTime;
    private float bgmTime;
    private string lastBGMName;
    private float lastVolume;

    public override void PlayMusic(string bgmName, float volume, float transition = 0.5f)
    {
        bgmTime = 0;
        audioManager.PlayMusic(bgmName, true, transition, volume, false, false);
        lastBGMName = bgmName;
        lastVolume = volume;
    }
    void Update()
    {
        if(bgmTime > bgmLoopTime)
        {
            PlayMusic(lastBGMName, lastVolume, 0.5f);
        }
    }
}
