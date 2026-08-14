using System.Collections;
using DG.Tweening;
using UnityEngine;
using SimpleLocalization;
using SimpleSaveSystem;
using UnityEngine.UI;

public class UI_Setting : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float languageSwapStep = 0.2f;
    [SerializeField] private Button[] langBtns;

    private float languageSwapTime;
    
    public bool isSettingOpen { get; private set; }

    void OnEnable()
    {
        languageSwapTime = Time.realtimeSinceStartup;
    }
    public void Btn_OnReturn()
    {
        SwitchSetting(false);
        SaveManager.SaveGameState(0);
    }
    public void Btn_OnNextLanguage()
    {
        if (Time.realtimeSinceStartup > languageSwapTime + languageSwapStep)
        {
            StartCoroutine(coroutineBtnRefresh());
            languageSwapTime = Time.realtimeSinceStartup;
            LocalizeManager.NextLocale();
        }
        else
        {
            Debug.LogWarning("Step Time not enough");
        }
    }
    public void Btn_OnPreviousLanguage()
    {
        if (Time.realtimeSinceStartup > languageSwapTime + languageSwapStep)
        {
            StartCoroutine(coroutineBtnRefresh());
            languageSwapTime = Time.realtimeSinceStartup;
            LocalizeManager.PreviousLocale();
        }
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

    IEnumerator coroutineBtnRefresh()
    {
        foreach (var btn in langBtns)
        {
            btn.interactable = false;
            btn.targetGraphic.raycastTarget = false;
        }

        yield return new WaitForSecondsRealtime(languageSwapStep);

        foreach (var btn in langBtns)
        {
            btn.interactable = true;
            btn.targetGraphic.raycastTarget = true;
        }
    }
}