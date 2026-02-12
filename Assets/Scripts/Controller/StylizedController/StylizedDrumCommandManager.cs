using System;
using UnityEngine;

public class StylizedDrumCommandManager : CommandManager<StylizedDrumController>
{
    public StylizedDrumCommandManager(StylizedDrumController context) : base(context)
    {
    }
}
public class DrumCommand : Command<StylizedDrumController>{}
public class PlayBeatCommand : DrumCommand
{
    private string sfxKey;
    private float volume;
    private Action onBeatPlayed;
    public PlayBeatCommand(string sfxKey, float volume=1f, Action onBeatPlayed = null)
    {
        this.sfxKey = sfxKey;
        this.volume = volume;
        this.onBeatPlayed = onBeatPlayed;
    }
    internal override void CommandUpdate(StylizedDrumController context)
    {
        context.PlayBeats(sfxKey, volume);
        onBeatPlayed?.Invoke();
        SetStatus(CommandStatus.Success);
    }
}
public class ContinuousBeat : DrumCommand
{
    private string sfxKey;
    private float volume;
    private AudioSource audioSource;
    public ContinuousBeat(string sfxKey, float volume, AudioSource audioSource)
    {
        this.sfxKey = sfxKey;
        this.volume = volume;
        this.audioSource = audioSource;
    }
    internal override void CommandUpdate(StylizedDrumController context)
    {
        context.PlayContinuousBeat(sfxKey, volume, audioSource);
        SetStatus(CommandStatus.Success);
    }
}
