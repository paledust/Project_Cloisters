using SimpleAudioSystem;
using UnityEngine;

public class StylizedDrumController : Singleton<StylizedDrumController>
{
    [SerializeField] private string ambKey;
    [SerializeField] private int BPM = 105;
    private StylizedDrumCommandManager stylizedDrumCommandManager;

    private double temple;
    private double beat;

    void Start()
    {
        stylizedDrumCommandManager = new StylizedDrumCommandManager(this);
        AudioManager.Instance.PlayAmbience(ambKey, true, 1, false);
        temple = BPM/60.0f * 8;
        beat = 0;
    }
    void Update()
    {
        beat += temple * Time.deltaTime;
        if (beat >= 1)
        {
            beat -= 1;
            stylizedDrumCommandManager.UpdateCommand();
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