using UnityEngine;

namespace SimpleAudioSystem
{
    public abstract class AudioData_SO : ScriptableObject
    {
        internal string AudioKey => this.name;
        internal abstract AudioClip GetAudioClip();
    }
}