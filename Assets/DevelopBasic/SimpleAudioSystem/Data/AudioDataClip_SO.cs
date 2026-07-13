using UnityEngine;

namespace SimpleAudioSystem
{
    [CreateAssetMenu(fileName = "AudioDataClip", menuName = "DevelopBasic/AudioSystem/AudioDataClip")]
    public class AudioDataClip_SO : AudioData_SO
    {
        [SerializeField] private AudioClip audioClip;
        internal override AudioClip GetAudioClip() => audioClip;
    }
}
