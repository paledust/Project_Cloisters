using System.Collections.Generic;
using UnityEngine;

namespace SimpleAudioSystem
{
    [CreateAssetMenu(fileName = "AudioDataCollection_SO", menuName = "DevelopBasic/AudioSystem/AudioDataCollection_SO")]
    public class AudioDataCollection_SO : ScriptableObject
    {
        public List<AudioData_SO> bgm_info_list;
        public List<AudioData_SO> amb_info_list;
        public List<AudioData_SO> sfx_info_list;

        private Dictionary<string, AudioData_SO> bgmDict;
        private Dictionary<string, AudioData_SO> ambDict;
        private Dictionary<string, AudioData_SO> sfxDict;

        void OnEnable()
        {
            bgmDict = new Dictionary<string, AudioData_SO>();
            ambDict = new Dictionary<string, AudioData_SO>();
            sfxDict = new Dictionary<string, AudioData_SO>();
            foreach(var data in bgm_info_list)
                bgmDict.TryAdd(data.AudioKey, data);
            foreach(var data in amb_info_list)
                ambDict.TryAdd(data.AudioKey, data);
            foreach(var data in sfx_info_list)
                sfxDict.TryAdd(data.AudioKey, data);
        }
        public AudioClip GetBGMClipByName(string audioKey)
        {
            if(bgmDict.TryGetValue(audioKey, out var data))
                return data.GetAudioClip();
            return null;
        }
        public AudioClip GetAMBClipByName(string audioKey)
        {
            if(ambDict.TryGetValue(audioKey, out var data))
                return data.GetAudioClip();
            return null;
        }
        public AudioClip GetSFXClipByName(string audioKey){
            if(sfxDict.TryGetValue(audioKey, out var data))
                return data.GetAudioClip();
            return null;
        }
    }
}
