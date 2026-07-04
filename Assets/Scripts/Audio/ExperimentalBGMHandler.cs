using UnityEngine;

public class ExperimentalBGMHandler : BGMHandler
{
    private enum BGMState
    {
        Stop,
        Playing,
        Fading,
        Silent
    }
    [SerializeField, ShowOnly] private BGMState state;
    [SerializeField] private float bgmDuration;
    [SerializeField] private float silcenDuration;
    [SerializeField] private float fadeDuration;
    [SerializeField] private bool resumeOnTime;
    private string lastBGMName;
    private float bgmTime;
    private float lastVolume;

    protected override void OnInit()
    {
        state = BGMState.Stop;
        bgmTime = 0;    
    }
    public override void PlayMusic(string bgmName, float volume, float transition = 0.5f)
    {
        if(state!=BGMState.Playing)
        {
            bgmTime = 0;
            state = BGMState.Playing;
            audioManager.PlayMusic(bgmName, true, transition, volume, false, false);
        }
    }
    void Update()
    {
        switch(state)
        {
            case BGMState.Playing:
                bgmTime += Time.deltaTime;
                if(bgmTime > bgmDuration)
                {
                    state = BGMState.Fading;
                    bgmTime = 0;
                    PlayMusic(lastBGMName, lastVolume, 0.5f);
                }
                break;
            case BGMState.Fading:
                bgmTime += Time.deltaTime;
                if(bgmTime > fadeDuration)
                {
                    state = BGMState.Silent;
                    bgmTime = 0;
                }
                break;
            case BGMState.Silent:
                bgmTime += Time.deltaTime;
                if(bgmTime > silcenDuration)
                {
                    state = BGMState.Playing;
                    bgmTime = 0;
                    PlayMusic(lastBGMName, lastVolume, 0.5f);
                }
                break;
            default:
                break;
        }
    }
}
