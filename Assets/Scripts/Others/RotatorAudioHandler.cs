using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

[RequireComponent(typeof(Clickable_ObjectRotator))]
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

    private Clickable_ObjectRotator rotator;

    void Awake()
    {
        rotator = GetComponent<Clickable_ObjectRotator>();
    }
    void Update()
    {
        float speed = Mathf.Abs(rotator.m_angularSpeed);
        if(speed > angularSpeedThreshold)
        {
            if(!audioSource.isPlaying)
                AudioManager.Instance.PlaySFXLoop(audioSource, sfxRotating, 0, 0);
            audioSource.volume = Mathf.Lerp(audioSource.volume, Mathf.Clamp01((speed-angularSpeedThreshold)*angularSpeedToVolume)*volumeScale, Time.deltaTime*volumeLerpSpeed);
        }
        else
        {
            if(audioSource.isPlaying)
                audioSource.volume = Mathf.Lerp(audioSource.volume, 0, Time.deltaTime*volumeLerpSpeed);
            if(audioSource.volume < 0.01f)
                audioSource.Stop();
        }        
    }
}
