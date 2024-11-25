using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;

[DisplayName("AmbientFade Marker"), CustomStyle("AmbientFadeMarker")]
public class AmbientCrossFadeMarker : Marker, INotification, INotificationOptionProvider
{
    public string ambientName;
    [Range(0,1)] public float volume;
    public float transitionTime = 1;
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
