using DG.Tweening;
using UnityEngine;

public class UI_Setting : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    public bool isSettingOpen { get; private set; }

    public void Btn_OnReturn() => SwitchSetting(false);
    public void SwitchSetting(bool isOn)
    {
        if(isOn)
        {
            canvasGroup.gameObject.SetActive(true);
            canvasGroup.interactable = true;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.DOFade(1, 0.15f).SetUpdate(true);
            isSettingOpen = true;
        }
        else
        {
            canvasGroup.interactable = false;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.DOFade(0, 0.15f).OnComplete(()=>gameObject.SetActive(false)).SetUpdate(true);
            isSettingOpen = false;
        }
    }
}