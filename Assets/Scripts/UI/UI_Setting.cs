using DG.Tweening;
using UnityEngine;

public class UI_Setting : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;

    public void Btn_OnReturn()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.DOFade(0, 0.15f).OnComplete(()=>gameObject.SetActive(false)).SetUpdate(true);
    }
}