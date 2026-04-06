using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_Manager : Singleton<UI_Manager>
{
    [SerializeField] private CursorState_SO cursorState_SO;
[Header("Custom Cursor")]
    [SerializeField] private CanvasGroup customCursor;
[Header("Cursor Sprite")]
    [SerializeField] private Image imgCursor;

    private CURSOR_STATE currentCursorState = CURSOR_STATE.DEFAULT;
    private CoroutineExcuter cursorChanger;

    protected override void Awake()
    {
        base.Awake();
        imgCursor.color = Color.white;

        cursorChanger = new CoroutineExcuter(this);
        UpdateCursorState(currentCursorState);

        EventHandler.E_OnTransitionBegin += TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd += TransitionEndHandler;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandler.E_OnTransitionBegin -= TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd -= TransitionEndHandler;
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
        switch(currentCursorState){
            case CURSOR_STATE.DEFAULT:
                switch(newState){
                    case CURSOR_STATE.HOVER:
                        cursorChanger.Excute(coroutineChangeCursor(0.8f, 1.2f, 0.2f));
                        break;
                    case CURSOR_STATE.DRAG:
                        cursorChanger.Excute(coroutineChangeCursor(0.2f, 0f, 0.2f));
                        break;
                }
                break;
            case CURSOR_STATE.HOVER:
                switch(newState){
                    case CURSOR_STATE.DEFAULT:
                        cursorChanger.Excute(coroutineChangeCursor(0.5f, 1f, 0.2f));
                        break;
                    case CURSOR_STATE.DRAG:
                        cursorChanger.Excute(coroutineChangeCursor(0.2f, 0f, 0.2f));
                        break;
                }
                break;
            case CURSOR_STATE.DRAG:
                switch(newState){
                    case CURSOR_STATE.DEFAULT:
                        cursorChanger.Excute(coroutineChangeCursor(0.5f, 1f, 0.2f));
                        break;
                    case CURSOR_STATE.HOVER:
                        cursorChanger.Excute(coroutineChangeCursor(0.8f, 1.2f, 0.2f));
                        break;
                }
                break;
        }
        currentCursorState = newState;
    }
    void TransitionBeginHandler(){
        cursorChanger.Excute(coroutineChangeCursor(0f, 1f, 0.5f));
    }
    void TransitionEndHandler(){
        cursorChanger.Excute(coroutineChangeCursor(0.5f, 1f, 0.5f));
    }
    public void HideCursor()
    {
        cursorChanger.Excute(coroutineChangeCursor(0f, 1f, 0.2f));
    }
    public void ShowCursor()
    {
        cursorChanger.Excute(coroutineChangeCursor(0.5f, 1f, 0.5f));
    }
    IEnumerator coroutineChangeCursor(float alpha, float size, float duration){
        float initSize = customCursor.transform.localScale.x;
        float initAlpha = customCursor.alpha;
        yield return new WaitForLoop(duration, (t)=>{
            customCursor.alpha = Mathf.Lerp(initAlpha, alpha, EasingFunc.Easing.SmoothInOut(t));
            customCursor.transform.localScale = Vector3.one * Mathf.Lerp(initSize, size, EasingFunc.Easing.SmoothInOut(t));
        });
    }
}