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
    public PlayBeatCommand(string sfxKey, float volume=1f)
    {
        this.sfxKey = sfxKey;
        this.volume = volume;
    }
    internal override void CommandUpdate(StylizedDrumController context)
    {
        context.PlayBeats(sfxKey, volume);
        SetStatus(CommandStatus.Success);
    }
}
