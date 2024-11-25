using System.ComponentModel;
using UnityEngine;
using UnityEngine.Timeline;

[DisplayName("Note Mark"),CustomStyle("NoteMarker")]
public class NoteMark : Marker
{
    [TextArea]
    public string Note;
}