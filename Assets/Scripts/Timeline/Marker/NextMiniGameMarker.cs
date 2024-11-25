using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[DisplayName("NextMiniGame Marker"), CustomStyle("NextMiniGameMarker")]
public class NextMiniGameMarker : Marker, INotification, INotificationOptionProvider
{
    public bool retroactive;
    public bool emitOnce;
    public PropertyName id{get{return new PropertyName();}}
    NotificationFlags INotificationOptionProvider.flags{
        get
        {
            return (retroactive ? NotificationFlags.Retroactive : default(NotificationFlags)) |
                (emitOnce ? NotificationFlags.TriggerOnce : default(NotificationFlags));
        }
    }
}
