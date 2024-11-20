using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    [SerializeField] private CursorState_SO cursorState_SO;

    private bool IsInTransition;
    private PlayerController currentPlayer;
    private CURSOR_STATE currentCursorState = CURSOR_STATE.DEFAULT;
    
    public bool m_canControl{get{return !IsInTransition;}}

    void HideCursor()=>Cursor.visible = false;
    void ShowCursor()=>Cursor.visible = true;
    protected override void Awake(){
        base.Awake();

        var data = cursorState_SO.GetCursorStateData(currentCursorState);
        Cursor.SetCursor(data.texture, data.offset, CursorMode.Auto);
        EventHandler.E_AfterLoadScene += FindPlayer;
        EventHandler.E_OnTransitionBegin += TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd += TransitionEndHandler;
    }
    void Start(){
        FindPlayer();
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandler.E_AfterLoadScene -= FindPlayer;
        EventHandler.E_OnTransitionBegin -= TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd -= TransitionEndHandler;
    }
    void TransitionBeginHandler(){
        IsInTransition = true;
        currentPlayer.CheckControllable();
    }
    void TransitionEndHandler(){
        IsInTransition = false;
        currentPlayer.CheckControllable();
    }
    void FindPlayer(){
        currentPlayer = FindObjectOfType<PlayerController>();
    }
    public void UpdateCursorState(CURSOR_STATE newState){
        if(currentCursorState != newState){
            currentCursorState = newState;
            var data = cursorState_SO.GetCursorStateData(currentCursorState);
            Cursor.SetCursor(data.texture, data.offset, CursorMode.Auto);
        }
    }
}
