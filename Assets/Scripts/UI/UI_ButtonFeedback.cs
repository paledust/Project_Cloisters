using SimpleAudioSystem;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class UI_ButtonFeedback : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerExitHandler
{
    [SerializeField] private AudioData_SO sfxClick;
    [SerializeField] private float volumeClick;
    [SerializeField] private AudioData_SO sfxHover;
    [SerializeField] private float volumeHover;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI text;
    [SerializeField] private bool enableHoverUnderline;

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFXUI(sfxHover?.AudioKey, volumeHover);
        if(text!=null && enableHoverUnderline)
        {
            text.fontStyle |= FontStyles.Underline;
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        if(text!=null && enableHoverUnderline)
        {
            text.fontStyle &= ~FontStyles.Underline;
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySFXUI(sfxClick?.AudioKey, volumeClick);
    }
}
