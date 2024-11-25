using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using SimpleAudioSystem;

public class AmbientCrossFadeMarker_Receiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context){
        var mark = notification as AmbientCrossFadeMarker;
        if (mark == null)
            return;
        AudioManager.Instance.PlayAmbience(mark.ambientName, true, mark.transitionTime, mark.volume);
    }
}
