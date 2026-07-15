using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;

public class TubeForceTrigger : MonoBehaviour
{
    [SerializeField] private float forceScale = 24;
    [SerializeField] private float correctForceScale = 1;
    [SerializeField] private ParticleSystem vfxAttraction;
    [SerializeField] private EvocativeTube evocativeTube;

    [Header("Audio")]
    [SerializeField] private AudioSource vaccumAudio;
    [SerializeField] private AudioData_SO sfxVaccumLoop;
    [SerializeField] private AudioData_SO sfxVaccumOn;
    [SerializeField] private float maxVolume;
    [SerializeField] private float fadeInTime;
    [SerializeField] private float fadeOutTime;
    [SerializeField] private float minPitch = 0.8f;

    private Rigidbody ballRigid;
    private Sequence audioTween;
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BounceBall ball))
        {
            ballRigid = ball.GetComponent<Rigidbody>();
            vfxAttraction.Play();
            evocativeTube.StartTubeShake();

            if(!vaccumAudio.isPlaying)
            {
                vaccumAudio.pitch = 0.8f;
                AudioManager.Instance.PlaySFX(sfxVaccumOn.AudioKey, maxVolume);
                AudioManager.Instance.PlaySFXLoop(vaccumAudio, sfxVaccumLoop.AudioKey, 0, 0);
            }
            
            if(audioTween!=null)
            {
                audioTween.Kill();
            }
            audioTween = DOTween.Sequence();
            audioTween.Join(vaccumAudio.DOFade(maxVolume, fadeInTime))
                    .Join(vaccumAudio.DOPitch(1, fadeInTime));
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out BounceBall ball))
        {
            ballRigid = null;
            vfxAttraction.Stop();
            evocativeTube.StopTubeShake();

            if(audioTween!=null)
                audioTween.Kill();
            audioTween = DOTween.Sequence();
            audioTween.Join(vaccumAudio.DOFade(0, fadeOutTime))
                    .Join(vaccumAudio.DOPitch(minPitch, fadeOutTime))
                    .OnComplete(()=>vaccumAudio.Stop());
        }
    }
    void FixedUpdate()
    {
        if(ballRigid != null)
        {
            Vector2 pos = ballRigid.position;
            ballRigid.AddForce(forceScale * transform.up + correctForceScale*forceScale*transform.right * Vector2.Dot((Vector2)transform.position-pos, transform.right));
        }
    }
}
