using UnityEngine;
using UnityEngine.UI;   
using DG.Tweening;
using System;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField] private CursorState_SO cursorState_SO;
[Header("Custom Cursor")]
    [SerializeField] private CanvasGroup customCursor;
[Header("Cursor Sprite")]
    [SerializeField] private Image imgCursor;

    private CURSOR_STATE currentCursorState = CURSOR_STATE.DEFAULT;
    private bool cursorVisible = true;
    private Sequence cursorTween;

    protected override void Awake()
    {
        base.Awake();
        imgCursor.color = Color.white;
        cursorVisible = true;

        UpdateCursorState(currentCursorState);

        EventHandler.E_OnTransitionBegin += HideCursor;
        EventHandler.E_OnTransitionEnd += ShowCursor;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandler.E_OnTransitionBegin -= HideCursor;
        EventHandler.E_OnTransitionEnd -= ShowCursor;
    }
    public void ChangeCursorColor(bool isWhite)
    {
        imgCursor.DOKill();
        imgCursor.DOColor(isWhite? Color.white : Color.black, 0.2f);
    }
    public void UpdateCursorPos(Vector2 scrPos){
        customCursor.transform.position = scrPos;
    }
    public void UpdateCursorState(CURSOR_STATE newState){
        if(!cursorVisible)
            return;
        switch(currentCursorState){
            case CURSOR_STATE.DEFAULT:
                switch(newState){
                    case CURSOR_STATE.HOVER:
                        ChangeCursorVisual(0.8f, 1.2f, 0.2f);
                        break;
                    case CURSOR_STATE.DRAG:
                        ChangeCursorVisual(0.2f, 0f, 0.2f);
                        break;
                }
                break;
            case CURSOR_STATE.HOVER:
                switch(newState){
                    case CURSOR_STATE.DEFAULT:
                        ChangeCursorVisual(0.5f, 1f, 0.2f);
                        break;
                    case CURSOR_STATE.DRAG:
                        ChangeCursorVisual(0.2f, 0f, 0.2f);
                        break;
                }
                break;
            case CURSOR_STATE.DRAG:
                switch(newState){
                    case CURSOR_STATE.DEFAULT:
                        ChangeCursorVisual(0.5f, 1f, 0.2f);
                        break;
                    case CURSOR_STATE.HOVER:
                        ChangeCursorVisual(0.8f, 1.2f, 0.2f);
                        break;
                }
                break;
        }
        currentCursorState = newState;
    }
    void HideCursor(){
        cursorVisible = false;
        ChangeCursorVisual(0f, 1f, 0.2f);
    }
    public void ShowCursor()
    {
        cursorVisible = true;
        ChangeCursorVisual(0.5f, 1f, 0.5f);
    }
    void ChangeCursorVisual(float alpha, float size, float duration, Action completeCallback = null)
    {
        if(cursorTween!=null)
            cursorTween.Kill();
        cursorTween = DOTween.Sequence();
        cursorTween.Join(customCursor.DOFade(alpha, duration)).SetEase(Ease.InOutQuad)
            .Join(customCursor.transform.DOScale(size, duration)).SetEase(Ease.InOutQuad)
            .OnComplete(()=>completeCallback?.Invoke());
    }
}