using SimpleAudioSystem;
using UnityEngine;

public class StylizedDrumController : Singleton<StylizedDrumController>
{
    [SerializeField] private int BPM = 105;
    [SerializeField] private bool playBeats;
    [SerializeField] private int seg = 4;
    [SerializeField] private int beatsGap = 3;

    private StylizedDrumCommandManager stylizedDrumCommandManager;

    private double nextBeat;
    private double beatGap;
    private double lastpcmTime;

    void Start()
    {
        stylizedDrumCommandManager = new StylizedDrumCommandManager(this);
        beatGap = 60.0d/(0d+BPM * seg);
        nextBeat = 0;
        lastpcmTime = 0;
    }
    void Update()
    {
        double pcmTime = AudioManager.Instance.GetAmbiencePCMTime();
        if(lastpcmTime > pcmTime)
        {
            nextBeat = 0;
        }
        if (pcmTime >= nextBeat)
        {
            nextBeat += beatGap;
            if(nextBeat > AudioManager.Instance.GetAmbienceLength())
            {
                nextBeat = 0;
            }
            stylizedDrumCommandManager.UpdateCommand();
            if(playBeats)
                AudioManager.Instance.PlaySoundEffect("group_click",1);
            EventHandler.Call_OnDrumBeat();
        }
        lastpcmTime = pcmTime;
    }
    public void PlayBeats(string sfxKey, float volume = 1)
    {
        AudioManager.Instance.PlaySoundEffect(sfxKey, volume);
    }
    public void QueueBeat(string sfxKey, float volume = 1)
    {
        var playBeatCommand = new PlayBeatCommand(sfxKey, volume);
        stylizedDrumCommandManager.AddCommand(playBeatCommand);
    }
}