using DG.Tweening;
using UnityEngine;
using SimpleLocalization;
using SimpleSaveSystem;
using TMPro;

public class UI_Setting : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float languageSwapStep = 0.2f;

    private float languageSwapTime;
    
    public bool isSettingOpen { get; private set; }

    void OnEnable()
    {
        languageSwapTime = Time.time;
    }
    public void Btn_OnReturn()
    {
        SwitchSetting(false);
        SaveManager.SaveGameState(0);
    }
    public void Btn_OnNextLanguage()
    {
        if(Time.time<languageSwapTime+languageSwapStep)
            return;
        languageSwapTime = Time.time;
        LocalizeManager.NextLocale();
    }
    public void Btn_OnPreviousLanguage()
    {
        if(Time.time<languageSwapTime+languageSwapStep)
            return;
        languageSwapTime = Time.time;
        LocalizeManager.PreviousLocale();
    }
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