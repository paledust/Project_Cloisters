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

    private float stateTime;
    private float lastVolume;

    protected override void OnInit()
    {
        state = BGMState.Stop;
        stateTime = 0;
    }
    public override void PlayMusic(string bgmName, float volume, float transition = 0.5f)
    {
        if(state!=BGMState.Playing)
        {
            stateTime = 0;
            state = BGMState.Playing;
            audioManager.PlayMusic(bgmName, true, transition, volume, false);
        }
    }
    void Update()
    {
        switch(state)
        {
            case BGMState.Playing:
                stateTime += Time.deltaTime;
                if(stateTime > bgmDuration)
                {
                    state = BGMState.Fading;
                    stateTime = 0;
                    audioManager.FadeMusic(0, fadeDuration, false);
                }
                break;
            case BGMState.Fading:
                stateTime += Time.deltaTime;
                if(stateTime > fadeDuration)
                {
                    state = BGMState.Silent;
                    stateTime = 0;
                }
                break;
            case BGMState.Silent:
                stateTime += Time.deltaTime;
                if(stateTime > silcenDuration)
                {
                    state = BGMState.Playing;
                    stateTime = 0;
                    audioManager.FadeMusic(lastVolume, 0.5f, false);
                }
                break;
            default:
                break;
        }
    }
}
