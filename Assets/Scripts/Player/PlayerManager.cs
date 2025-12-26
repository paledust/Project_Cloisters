using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{
    private bool IsInTransition;
    private PlayerController currentPlayer;
    
    public bool m_canControl => !IsInTransition;
    public bool m_isHovering => currentPlayer.m_hoveringInteractable!=null;

    void HideCursor()=>Cursor.visible = false;
    void ShowCursor()=>Cursor.visible = true;
    protected override void Awake(){
        base.Awake();

        EventHandler.E_AfterLoadScene += FindPlayer;
        EventHandler.E_OnTransitionBegin += TransitionBeginHandler;
        EventHandler.E_OnTransitionEnd += TransitionEndHandler;
        EventHandler.E_OnFlushInput += FlashInputHandler;
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
        EventHandler.E_OnFlushInput -= FlashInputHandler;
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
    public Vector3 GetCursorWorldPos(float depth)=>currentPlayer.GetCursorWorldPoint(depth);
    public void UpdateCursorState(CURSOR_STATE newState)=>UI_Manager.Instance.UpdateCursorState(newState);
}