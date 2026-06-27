using UnityEngine;

namespace SimpleAudioSystem
{
    public class AudioDataClip_SO : AudioData_SO
    {
        [SerializeField] private AudioClip audioClip;
        internal override AudioClip GetAudioClip() => audioClip;
    }
}
