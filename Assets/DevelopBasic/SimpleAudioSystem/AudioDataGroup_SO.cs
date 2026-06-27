using UnityEngine;

namespace SimpleAudioSystem
{
    public class AudioDataGroup_SO : AudioData_SO
    {
        [SerializeField] private AudioClip[] audioClips;
        internal override AudioClip GetAudioClip()
        {
            return audioClips[Random.Range(0, audioClips.Length)];
        }
    }
}
