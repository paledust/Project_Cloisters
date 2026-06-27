using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;

namespace SimpleAudioSystem{
    public class AudioManager : Singleton<AudioManager>
    {
        public enum AudioType{BGM, AMB, SFX}
        [SerializeField] private AudioDataCollection_SO audioCollection;
    [Header("Audio source")]
        [SerializeField] private AudioSource sfx_trigger;
        [SerializeField] private AudioSource ambience_loop;
        [SerializeField] private AudioSource music_loop;
    [Header("Audio mixer")]
        [SerializeField] private AudioMixer mainMixer;
        [SerializeField] private AudioMixerSnapshot[] mixerSnapShots;

        private bool ambience_crossfading = false;
        private bool music_crossfading = false;

        public string current_music_name{get; private set;} = string.Empty;
        public string current_ambience_name{get; private set;} = string.Empty;

        private const string masterVolumeName = "MasterVolume";
        private CoroutineExcuter ambFader;
        private CoroutineExcuter musicFader;

        protected override void Awake()
        {
            base.Awake();
        }

        #region Sound Play
        public void PlayMusic(string audio_name, float volume = 1){
            current_music_name = audio_name;
            if(audio_name == string.Empty) music_loop.Stop();

            music_loop.clip = audioCollection.GetBGMClipByName(audio_name);
            if(music_loop.clip!=null){
                music_loop.volume = volume;
                music_loop.Play();
            }
        }
        public void PlayMusicDelayed(string audio_name, float delayed)
        {
            if(audio_name == string.Empty)
                return;
            music_loop.PlayDelayed(delayed);
        }
        public void PlayMusic(string audio_name, bool startOver, float transitionTime, float volume = 1, bool forcePlay = false){
        //If no audio name, fade out the ambience
            if(audio_name == string.Empty){
                FadeAudio(music_loop, 0, transitionTime, true);
                current_music_name = string.Empty;
                return;
            }
        //If the audio name is the same, only fade the volume to the target value
            if(current_music_name == audio_name){
                FadeAudio(music_loop, volume, transitionTime);
            }
            else{
                if(current_music_name == string.Empty || !music_loop.isPlaying){
                    music_loop.clip = audioCollection.GetBGMClipByName(audio_name);
                    if(music_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    music_loop.Play();
                    FadeAudio(music_loop, volume, transitionTime);
                }
                else{
                    if(music_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    CorssFadeMusic(audio_name, volume, startOver, transitionTime, forcePlay);
                }
                current_music_name = audio_name;
            }   
        }
        public void PlayAmbience(string audio_name, bool startOver, float transitionTime, float volume = 1, bool forcePlay = false){
        //If no audio name, fade out the ambience
            if(audio_name == string.Empty){
                FadeAudio(ambience_loop, 0, transitionTime, true);
                current_ambience_name = string.Empty;
            }
        //If the audio name is the same, only fade the volume to the target value
            if(current_ambience_name==audio_name){
                FadeAudio(ambience_loop, volume, transitionTime);
            }
            else{
                if(current_ambience_name == string.Empty || !ambience_loop.isPlaying){
                    ambience_loop.clip = audioCollection.GetAMBClipByName(audio_name);
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    ambience_loop.volume = volume;
                    ambience_loop.Play();
                }
                else{
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    CrossFadeAmbience(audio_name, volume, startOver, transitionTime, forcePlay);
                }
                current_ambience_name = audio_name;
            }            
        }
        public void PlayAmbience(string audio_name, bool startOver, float volume=1, bool forcePlay = false){
        //If no audio name, fade out the ambience
            if(audio_name == string.Empty){
                FadeAudio(ambience_loop, 0, 0.5f, true);
                current_ambience_name = string.Empty;
            }
        //If the audio name is the same, only fade the volume to the target value
            if(current_ambience_name==audio_name){
                if(volume>0 && !ambience_loop.isPlaying)
                {
                    ambience_loop.Play();
                }
                FadeAudio(ambience_loop, volume, 0.5f);
            }
            else{
                if(current_ambience_name == string.Empty || !ambience_loop.isPlaying){
                    ambience_loop.clip = audioCollection.GetAMBClipByName(audio_name);
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    ambience_loop.volume = volume;
                    ambience_loop.Play();
                }
                else{
                    if(ambience_loop.clip==null){
                        Debug.LogWarning("No clip found, nothing will be done for ambient");
                        return;
                    }
                    CrossFadeAmbience(audio_name, volume, startOver, 0.5f, forcePlay);
                }
                current_ambience_name = audio_name;
            }
        }
        public AudioClip PlaySFX(AudioSource targetSource, string clip_name, float volumeScale){
            if(string.IsNullOrEmpty(clip_name)) 
                return null;
            AudioClip clip = audioCollection.GetSFXClipByName(clip_name);
            if(clip!=null)
                targetSource.PlayOneShot(clip, volumeScale);
            else
                Debug.LogAssertion($"No Clip found:{clip_name}");
            return clip;
        }
        public AudioClip PlaySFX(string clip_name, float volumeScale) => PlaySFX(sfx_trigger, clip_name, volumeScale);
        
        public AudioClip PlaySFXLoop(AudioSource targetSource, string clip_name, float volumeScale, float transition = 1f, float time = 0){
            AudioClip clip = audioCollection.GetSFXClipByName(clip_name);
            targetSource.clip = clip;
            targetSource.loop = true;
            targetSource.time = time;
            targetSource.Play();
            if(transition>0)
                FadeAudio(targetSource, volumeScale, transition);
            else
                targetSource.volume = volumeScale;

            return clip;
        }
        public void PlaySFXWithPitch(AudioSource targetSource, string clip_name, float volumeScale, float pitch){
            targetSource.pitch = pitch;
            PlaySFX(targetSource, clip_name, volumeScale);
        }
        public void PlaySFX(AudioSource targetSource, string clip, float volumeScale, float delay, Action completeCallback=null)=>
            StartCoroutine(coroutineDelaySFX(targetSource, clip, volumeScale, delay, completeCallback));
        public void PlaySoundEffect_WithFinishCallback(AudioSource targetSource, string clip, float volumScale, Action finishCallback=null)=>
            StartCoroutine(coroutineSFX_WithFinishAction(targetSource, clip, volumScale, finishCallback));
        public void FadeInAndOutSoundEffect(AudioSource targetSource, string clip, float maxVolume, float duration, float fadeIn, float fadeOut)=>
            StartCoroutine(coroutineFadeInAndOutSFX(targetSource, clip, maxVolume, duration, fadeIn, fadeOut));
    #endregion

        #region Helper function
        public static void SwitchAudioListener(AudioListener from, AudioListener to){
            from.enabled = false;
            to.enabled = true;
        }
        public bool IsPlayAMB(string ambience_name){
            return ambience_loop.isPlaying && current_ambience_name == ambience_name;
        }
        public void FadeAmbience(float targetVolume, float transitionTime, bool StopOnFadeOut = false)=>FadeAudio(ambience_loop, targetVolume, transitionTime, StopOnFadeOut);
        public void FadeMusic(float targetVolume, float transitionTime, bool StopOnFadeOut = false)=>FadeAudio(music_loop, targetVolume, transitionTime, StopOnFadeOut);
        void FadeAudio(AudioSource m_audio, float targetVolume, float transitionTime, bool StopOnFadeOut = false){
            if(transitionTime<=0)
            {
                m_audio.volume = targetVolume;
                return;
            }
            StartCoroutine(coroutineFadeAudio(m_audio, targetVolume, transitionTime, StopOnFadeOut));
        }
        public void ChangeMasterVolume(float targetVolume){
            mainMixer.SetFloat(masterVolumeName, targetVolume);
        }
        void CrossFadeAmbience(string audio_name, float targetVolume, bool startOver, float transitionTime, bool forceCrossFade = false){
            if(!forceCrossFade && ambience_crossfading) return;
            if(ambFader==null) ambFader = new CoroutineExcuter(this);
            ambFader.Excute(coroutineCrossFadeAmbience(current_ambience_name, audio_name, targetVolume, startOver, transitionTime));
        }
        void CorssFadeMusic(string audio_name, float targetVolume, bool startOver, float transitionTime, bool forceCrossFade = false){
            if(!forceCrossFade && music_crossfading) return;
            if(musicFader==null) musicFader = new CoroutineExcuter(this);
            musicFader.Excute(coroutineCrossFadeMusic(current_music_name, audio_name, targetVolume, startOver, transitionTime));           
        }
    #endregion
        
        #region PCM Time
        public double GetAudioPCMTime(AudioSource audioSource)
        {
            if(audioSource.clip == null) 
                return 0;
            return audioSource.timeSamples / (double)audioSource.clip.frequency;
        }
        public double GetAmbienceLength(AudioSource audioSource)
        {
            if(audioSource.clip == null) 
                return 0;
            return audioSource.clip.length;
        }

        #endregion

        IEnumerator coroutineFadeInAndOutSFX(AudioSource m_audio, string clip, float maxVolume, float duration, float fadeIn, float fadeOut){
            AudioSource tempAudio = Instantiate(m_audio);
            tempAudio.name = "_TempSFX";
            tempAudio.volume = 0;
            tempAudio.loop   = true;
            tempAudio.clip   = audioCollection.GetSFXClipByName(clip);
            tempAudio.Play();

            yield return new WaitForLoop(fadeIn, (t)=>tempAudio.volume = Mathf.Lerp(0, maxVolume, t));
            yield return new WaitForSeconds(duration);
            yield return new WaitForLoop(fadeOut, (t)=>tempAudio.volume = Mathf.Lerp(maxVolume, 0, t));

            Destroy(tempAudio.gameObject);
        }
        IEnumerator coroutineDelaySFX(AudioSource m_audio, string clip, float volumeScale, float delay, Action completeCallback){
            yield return new WaitForSeconds(delay);
            PlaySFX(m_audio, clip, volumeScale);
            completeCallback?.Invoke();
        }
        IEnumerator coroutineSFX_WithFinishAction(AudioSource m_audio, string clip, float volumeScale, Action finishCallback){
            var playedClip = PlaySFX(m_audio, clip, volumeScale);
            yield return new WaitForSeconds(playedClip==null?0:playedClip.length);
            finishCallback?.Invoke();
        }
        IEnumerator coroutineCrossFadeAmbience(string from_clip, string to_clip, float targetVolume, bool startOver, float transitionTime){
            ambience_crossfading = true;

            yield return coroutineCrossFadeAudio(ambience_loop, from_clip, to_clip, targetVolume, startOver, transitionTime, AudioType.AMB);
            current_ambience_name = to_clip;

            ambience_crossfading = false;
        }
        IEnumerator coroutineCrossFadeMusic(string from_clip, string to_clip, float targetVolume, bool startOver, float transitionTime){
            music_crossfading = true;
            yield return coroutineCrossFadeAudio(music_loop, from_clip, to_clip, targetVolume, startOver, transitionTime, AudioType.BGM);
            current_music_name = to_clip;
            music_crossfading = false;
        }
        IEnumerator coroutineCrossFadeAudio(AudioSource targetSource, string from_clip, string to_clip, float targetVolume, bool startOver, float transitionTime, AudioType audioType){
            int audioTime = targetSource.timeSamples;
            if (from_clip != string.Empty)
            {
                AudioSource tempAudio = new GameObject($"[_Temp_{targetSource.name}]").AddComponent<AudioSource>();
                Destroy(tempAudio.gameObject, transitionTime+0.1f); //Schedule an auto Destruction;
                tempAudio.loop   = true;
                tempAudio.timeSamples = targetSource.timeSamples;
                tempAudio.volume = targetSource.volume;
                tempAudio.outputAudioMixerGroup = targetSource.outputAudioMixerGroup;
                tempAudio.loop   = targetSource.loop;
                tempAudio.pitch  = targetSource.pitch;
                tempAudio.clip   = targetSource.clip;
                tempAudio.Play();
                StartCoroutine(coroutineFadeAudio(tempAudio, 0, transitionTime));
            }

        //Get Audio clip based on type
            AudioClip targetClip;
            switch(audioType){
                case AudioType.SFX:
                    targetClip = audioCollection.GetSFXClipByName(to_clip);
                    break;
                case AudioType.AMB:
                    targetClip = audioCollection.GetAMBClipByName(to_clip);
                    break;
                case AudioType.BGM:
                    targetClip = audioCollection.GetBGMClipByName(to_clip);
                    break;
                default:
                    targetClip = audioCollection.GetAMBClipByName(to_clip);
                    break;
            }

        //Fade In Clip
            targetSource.clip = targetClip;
            targetSource.volume = 0;

            if(startOver) 
                targetSource.timeSamples = 0;
            else 
                targetSource.timeSamples = audioTime;

            targetSource.Play();
            yield return coroutineFadeAudio(targetSource, targetVolume, transitionTime);
        }
        IEnumerator coroutineFadeAudio(AudioSource source, float targetVolume, float transition, bool StopOnFadeOut = false){
            float initVolume = source.volume;
            yield return new WaitForLoop(transition, (t)=>{
                source.volume = Mathf.Lerp(initVolume, targetVolume, t);
            });
            yield return null;

            if(StopOnFadeOut && source.volume == 0) source.Stop();
        }
    }
}
