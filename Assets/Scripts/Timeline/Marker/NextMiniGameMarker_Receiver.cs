using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
public class NextMiniGameMarker_Receiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context){
        var mark = notification as NextMiniGameMarker;
        if (mark == null)
            return;
        
        EventHandler.Call_OnNextInteraction();
    }
}
