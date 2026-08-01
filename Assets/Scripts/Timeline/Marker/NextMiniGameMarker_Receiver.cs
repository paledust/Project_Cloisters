using UnityEngine;
using UnityEngine.Playables;
public class NextMiniGameMarker_Receiver : MonoBehaviour, INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context){
        var mark = notification as NextMiniGameMarker;
        if (mark == null)
            return;
        
        if(mark.autoUnloadLastInteraction)
            EventHandler.Call_OnUnloadCurrentInteraction();
        EventHandler.Call_OnNextInteraction();
    }
}
