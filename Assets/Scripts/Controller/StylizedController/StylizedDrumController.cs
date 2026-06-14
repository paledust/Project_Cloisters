using System;
using SimpleAudioSystem;
using UnityEngine;

public class StylizedDrumController : Singleton<StylizedDrumController>
{
    private enum BeatType
    {
        Balance = 0,
        WeakAndStrong = 1,
    }
    [SerializeField] private int BPM = 105;
    [SerializeField] private bool playBeats;
    [SerializeField] private int seg = 4;
    [SerializeField] private float weakBeatStrength = 0.2f;
    [SerializeField] private AudioSource sfxDrum;
    [SerializeField, ShowOnly] private BeatType beatType;

    private StylizedDrumCommandManager stylizedDrumCommandManager;

    private double nextBeat;
    private double beatGap;
    private double lastpcmTime;
    private int beatIndex;

    void Start()
    {
        beatType = BeatType.Balance;
        stylizedDrumCommandManager = new StylizedDrumCommandManager(this);
        beatGap = 60.0d/(0d+BPM * seg);
        nextBeat = 0;
        lastpcmTime = 0;
    }
    void Update()
    {
        double pcmTime = AudioManager.Instance.GetAudioPCMTime(sfxDrum);
        if(lastpcmTime > pcmTime)
        {
            nextBeat = 0;
        }
        if (pcmTime >= nextBeat)
        {
            nextBeat += beatGap;
            if(nextBeat > AudioManager.Instance.GetAmbienceLength(sfxDrum))
                nextBeat = 0;
            
            stylizedDrumCommandManager.UpdateCommand();
            if(playBeats)
                AudioManager.Instance.PlaySFX("group_click", 1);
            EventHandler.Call_OnDrumBeat();
            beatIndex++;
            beatIndex %= seg;
        }
        lastpcmTime = pcmTime;
    }
    public void EvolveBeatType()
    {
        beatType = BeatType.WeakAndStrong;
    }
    public void PlayBeats(string sfxKey, float volume = 1, bool volumeWithTempo = true)
    {
        float strength = 1;
        if(volumeWithTempo)
        {
            switch(beatType)
            {
                case BeatType.Balance:
                    break;
                case BeatType.WeakAndStrong:
                    strength = beatIndex==0?1:weakBeatStrength;
                    break;
            }
        }
        AudioManager.Instance.PlaySFX(sfxKey, volume*strength);
    }
    public PlayBeatCommand QueueBeat(string sfxKey, float volume = 1, Action beatCallback = null, bool volumeWithBeat = true)
    {
        var playBeatCommand = new PlayBeatCommand(sfxKey, volume, beatCallback, volumeWithBeat);
        stylizedDrumCommandManager.AddCommand(playBeatCommand);
        return playBeatCommand;
    }
    public PlayBeatCommand QueueBeat(string sfxKey, float volume, PlayBeatCommand drumCommand)
    {
        if(drumCommand.IsDetached)
            return QueueBeat(sfxKey, volume, null, false);
        else
        {
            var playBeatCommand = new PlayBeatCommand(sfxKey, volume);
            drumCommand.QueueCommand(playBeatCommand);
            return playBeatCommand;
        }
    }
}