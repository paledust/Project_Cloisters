using SimpleAudioSystem;
using UnityEngine;

public class RotatorAudioHandler : MonoBehaviour
{
[Header("Angular Config")]
    [SerializeField] private float angularSpeedThreshold = 10f;
    [SerializeField] private float angularSpeedToVolume = 10;
[Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField, Range(0, 1)] private float volumeScale = 1f;
    [SerializeField] private float volumeLerpSpeed = 5f;
    [SerializeField] private string sfxRotating;
[Header("Negative Speed")]
    [SerializeField] private bool separateNegativeSpeed = false;
    [SerializeField] private float negativeSpeedVolume = 0.5f;
    [SerializeField] private float negativePitch = 0.8f;

    private IRotator rotator;
    [SerializeField] private float audioTime;

    void Awake()
    {
        audioTime = 0;
        rotator = GetComponent<IRotator>();
    }
    void Update()
    {
        if(separateNegativeSpeed)
        {
            float speed = rotator.m_angularSpeed;
            if(speed > angularSpeedThreshold)
            {
                if(!audioSource.isPlaying)
                    AudioManager.Instance.PlaySFXLoop(audioSource, sfxRotating, 0, 0, audioTime);
                audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp01((speed-angularSpeedThreshold)*angularSpeedToVolume)*negativeSpeedVolume, Time.deltaTime*volumeLerpSpeed);
                audioSource.pitch = 1f;
            }
            else if(speed < -angularSpeedThreshold)
            {
                if(!audioSource.isPlaying)
                    AudioManager.Instance.PlaySFXLoop(audioSource, sfxRotating, 0, 0, audioTime);
                audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp01((-speed-angularSpeedThreshold)*angularSpeedToVolume)*negativeSpeedVolume, Time.deltaTime*volumeLerpSpeed);
                audioSource.pitch = negativePitch;
            }
        }
        else
        {
            float speed = Mathf.Abs(rotator.m_angularSpeed);
            if(speed > angularSpeedThreshold)
            {
                if(!audioSource.isPlaying)
                    AudioManager.Instance.PlaySFXLoop(audioSource, sfxRotating, 0, 0, audioTime);
                audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp01((speed-angularSpeedThreshold)*angularSpeedToVolume)*volumeScale, Time.deltaTime*volumeLerpSpeed);
            }
            else
            {
                if(audioSource.isPlaying)
                {
                    audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime*volumeLerpSpeed);
                    if(audioSource.volume < 0.01f)
                    {
                        audioTime = audioSource.time;
                        audioSource.Stop();
                    }
                }
            }        
        }
    }
}
