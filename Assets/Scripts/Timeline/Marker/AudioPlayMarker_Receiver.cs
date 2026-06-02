using UnityEngine;
using UnityEngine.Playables;
using SimpleAudioSystem;

public class AudioPlayMarker_Receiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context){
        var mark = notification as AudioPlayMarker;
        if (mark == null)
            return;
        switch(mark.audioType)
        {
            case AudioManager.AudioType.AMB:
                AudioManager.Instance.PlayAmbience(mark.audioKey, true, mark.transitionTime, mark.volume);
                break;  
            case AudioManager.AudioType.BGM:
                AudioManager.Instance.PlayMusic(mark.audioKey, true, mark.transitionTime, mark.volume);
                break;
            case AudioManager.AudioType.SFX:
                AudioManager.Instance.PlaySFX(mark.audioKey, mark.volume);
                break;
        }
    }
}
