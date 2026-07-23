using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    [SerializeField] private AudioData_SO sfxClick;
    [SerializeField] private float volumeClick;
    [SerializeField] private AudioData_SO sfxHover;
    [SerializeField] private float volumeHover;
    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFXUI(sfxHover?.AudioKey, volumeHover);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFXUI(sfxClick?.AudioKey, volumeClick);
    }
}
