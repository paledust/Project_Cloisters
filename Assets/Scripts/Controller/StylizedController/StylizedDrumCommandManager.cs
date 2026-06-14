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
    private bool volumeWithBeat;
    private Action onBeatPlayed;
    public PlayBeatCommand(string sfxKey, float volume=1f, Action onBeatPlayed = null, bool volumeWithBeat = true)
    {
        this.sfxKey = sfxKey;
        this.volume = volume;
        this.volumeWithBeat = volumeWithBeat;
        this.onBeatPlayed = onBeatPlayed;
    }
    internal override void CommandUpdate(StylizedDrumController context)
    {
        context.PlayBeats(sfxKey, volume, volumeWithBeat);
        onBeatPlayed?.Invoke();
        SetStatus(CommandStatus.Success);
    }
}