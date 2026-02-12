using System;
using SimpleAudioSystem;
using UnityEngine;

public class StylizedDrumController : Singleton<StylizedDrumController>
{
    [SerializeField] private int BPM = 105;
    [SerializeField] private bool playBeats;
    [SerializeField] private int seg = 4;

    private StylizedDrumCommandManager stylizedDrumCommandManager;

    private double nextBeat;
    private double beatGap;
    private double lastpcmTime;
    private

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
                AudioManager.Instance.PlaySoundEffect("group_click", 1);
            EventHandler.Call_OnDrumBeat();
        }
        lastpcmTime = pcmTime;
    }
    public void PlayBeats(string sfxKey, float volume = 1)
    {
        AudioManager.Instance.PlaySoundEffect(sfxKey, volume);
    }
    public void PlayContinuousBeat(string sfxKey, float volume = 1, AudioSource audioSource = null)
    {
        AudioManager.Instance.PlaySoundEffectLoop(audioSource, sfxKey, volume, 0.2f);
    }
    public PlayBeatCommand QueueBeat(string sfxKey, float volume = 1, Action beatCallback = null)
    {
        var playBeatCommand = new PlayBeatCommand(sfxKey, volume, beatCallback);
        stylizedDrumCommandManager.AddCommand(playBeatCommand);
        return playBeatCommand;
    }
    public PlayBeatCommand QueueBeat(string sfxKey, float volume, PlayBeatCommand drumCommand)
    {
        if(drumCommand.IsDetached)
            return QueueBeat(sfxKey, volume);
        else
        {
            var playBeatCommand = new PlayBeatCommand(sfxKey, volume);
            drumCommand.QueueCommand(playBeatCommand);
            return playBeatCommand;
        }
    }
}