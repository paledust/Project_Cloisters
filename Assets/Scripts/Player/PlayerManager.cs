using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private bool IsInTransition;
    private PlayerController currentPlayer;
    
    public bool m_canControl{get{return !IsInTransition;}}

    void HideCursor()=>Cursor.visible = false;
    void ShowCursor()=>Cursor.visible = true;
    protected override void Awake(){
        base.Awake();

        EventHandler.E_AfterLoadScene += FindPlayer;
        EventHandler.E_OnTransitionBegin += TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd += TransitionEndHandler;
        EventHandler.E_OnFlashInput += FlashInputHandler;
    }
    void Start(){
        FindPlayer();
    }
    void Update(){
        if(currentPlayer!=null){
            UI_Manager.Instance.UpdateCursorPos(currentPlayer.PointerScrPos);
        }
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandler.E_AfterLoadScene -= FindPlayer;
        EventHandler.E_OnTransitionBegin -= TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd -= TransitionEndHandler;
        EventHandler.E_OnFlashInput -= FlashInputHandler;
    }
    void TransitionBeginHandler(){
        IsInTransition = true;
        currentPlayer?.CheckControllable();
    }
    void TransitionEndHandler(){
        IsInTransition = false;
        currentPlayer?.CheckControllable();
    }
    void FindPlayer(){
        currentPlayer = FindObjectOfType<PlayerController>();
    }
    void FlashInputHandler(){
        currentPlayer?.ReleaseCurrentHolding();
    }
    public void UpdateCursorState(CURSOR_STATE newState)=>UI_Manager.Instance.UpdateCursorState(newState);
}
