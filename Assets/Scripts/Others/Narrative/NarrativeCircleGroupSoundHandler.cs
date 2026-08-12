using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class NarrativeCircleGroupSoundHandler : MonoBehaviour
{
    [SerializeField] private AudioSource narrativeSoundSource;
    
    [Header("SFX")]
    [SerializeField] private AudioData_SO sfxExplode;
    [SerializeField] private float explodeVolume = .25f;
    [SerializeField] private AudioData_SO sfxCollide;
    [SerializeField] private float collideVolume = .25f;
    [SerializeField] private float collisionStep = .25f;
    [SerializeField] private AudioData_SO sfxTextAppear;
    [SerializeField] private float textAppearVolume = .25f;
    private float _collisionSoundTime = 0;

    public void PlayCollisionSound(bool forcePlay = true)
    {
        bool shouldPlay = forcePlay || Time.time > (_collisionSoundTime+collisionStep);
        if (shouldPlay)
        {
            narrativeSoundSource.pitch = Random.Range(.95f, 1.05f);
            AudioManager.Instance.PlaySFX(narrativeSoundSource, sfxCollide.AudioKey, collideVolume);
            _collisionSoundTime = Time.time;
        }
    }
    public void PlayCircleExplode()=>AudioManager.Instance.PlaySFX(sfxExplode.AudioKey, explodeVolume);
    public void PlayTextAppear()=>AudioManager.Instance.PlaySFX(sfxTextAppear.AudioKey, textAppearVolume);
}
