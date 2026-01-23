using SimpleAudioSystem;
using UnityEngine;

public class StylizedDrumController : Singleton<StylizedDrumController>
{
    [SerializeField] private int BPM = 105;
    [SerializeField] private int seg = 4;
    [SerializeField] private bool playBeats;

    private StylizedDrumCommandManager stylizedDrumCommandManager;

    private double temple;
    private double beat;

    void Start()
    {
        stylizedDrumCommandManager = new StylizedDrumCommandManager(this);
        temple = 60.0f/(BPM * seg);
        beat = 0;
    }
    void Update()
    {
        double pcmTime = AudioManager.Instance.GetAmbiencePCMTime();
        if (Mathf.Abs((float)(pcmTime - beat)) >= temple)
        {
            beat = pcmTime;
            stylizedDrumCommandManager.UpdateCommand();
            if(playBeats)
                AudioManager.Instance.PlaySoundEffect("group_click",1);
        }
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