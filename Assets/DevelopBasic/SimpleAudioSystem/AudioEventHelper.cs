using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleAudioSystem
{
    public class AudioEventHelper : MonoBehaviour
    {
        [SerializeField, Range(0, 1)] private float volumeScale = 1;
        public void PlayAudio(AudioData_SO audioData)
        {
            AudioManager.Instance.PlaySFX(audioData.AudioKey, volumeScale);
        }
    }
}
