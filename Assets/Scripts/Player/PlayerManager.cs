using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private bool IsInTransition;
    private PlayerController currentPlayer;
    
    public bool m_canControl => !IsInTransition;
    public bool m_isHovering => currentPlayer.m_hoveringInteractable!=null;

    protected override void Awake(){
        base.Awake();

        EventHandler.E_OnTransitionBegin += TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd += TransitionEndHandler;
        EventHandler.E_OnFlushInput += FlashInputHandler;
    }
    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventHandler.E_OnTransitionBegin -= TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd -= TransitionEndHandler;
        EventHandler.E_OnFlushInput -= FlashInputHandler;
    }
    void TransitionBeginHandler(){
        IsInTransition = true;
        if(currentPlayer!=null) 
            currentPlayer.CheckControllable();
    }
    void TransitionEndHandler(){
        IsInTransition = false;
        if(currentPlayer!=null) 
            currentPlayer.CheckControllable();
    }

    void FlashInputHandler(){
        if(currentPlayer!=null) 
            currentPlayer.ReleaseCurrentHolding();
    }
    public Vector2 GetCursorScreenPos()=>currentPlayer.PointerScrPos;
    public Vector2 GetCursorDelta()=>currentPlayer.PointerDelta;
    public void UpdateCursorState(CURSOR_STATE newState)=>UI_Manager.Instance.UpdateCursorState(newState);
    public void RegisterPlayer(PlayerController player)
    {
        currentPlayer = player;
        currentPlayer.Init(UI_Manager.Instance);
    }
    public void UnregisterPlayer(PlayerController player){
        if(currentPlayer == player) 
            currentPlayer = null;
    }
}