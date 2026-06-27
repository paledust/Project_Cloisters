using UnityEngine;
using System;

namespace SimpleAudioSystem
{
    public class AudioDataGroup_SO : AudioData_SO
    {
        [SerializeField] private AudioClip[] audioClips;
        [NonSerialized] private int playIndex = 0;

        void OnEnable()
        {
            playIndex = 0;
            Service.Shuffle(ref audioClips);
        }
        internal override AudioClip GetAudioClip()
        {
            var clip = audioClips[playIndex];
            playIndex ++;
            if(playIndex >= audioClips.Length)
            {
                playIndex = 0;
                Service.Shuffle(ref audioClips);
            }
            return clip;
        }
    }
}