using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerInput input;
[Header("Audio")]
    [SerializeField] private AudioSource playerAudio;

    public Basic_Clickable m_hoveringInteractable{get; private set;} //The hovering interactable.
    public Basic_Clickable m_holdingInteractable{get; private set;} //Currently holding interactable.
    public Vector2 PointerScrPos{get; private set;}
    public Vector2 PointerDelta{get; private set;}

    private Vector3 hoverPos;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;    
    }

    // Update is called once per frame
    void Update()
    {
        if(m_holdingInteractable==null)
        {
            Ray ray = mainCam.ScreenPointToRay(PointerScrPos);
            if(Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, 1<<Service.InteractableLayer)){
                Basic_Clickable hit_Interactable = hit.collider.GetComponent<Basic_Clickable>();
                hoverPos = hit.point;
                if(hit_Interactable!=null){
                    if(m_hoveringInteractable != hit_Interactable) {
                        if(m_hoveringInteractable!=null) m_hoveringInteractable.OnExitHover();

                        m_hoveringInteractable = hit_Interactable;
                        if(m_hoveringInteractable.m_isInteractable) m_hoveringInteractable.OnHover(this);
                        if(!m_holdingInteractable) PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.HOVER);
                    }
                }
                else{
                    ClearHoveringInteractable();
                }
            }
            else{
                ClearHoveringInteractable();
            }
        }
        else{
            m_holdingInteractable.ControllingUpdate(this);
        }
    }

#region Handle Interactable
    void ClearHoveringInteractable(){
        if(m_hoveringInteractable != null){
            m_hoveringInteractable.OnExitHover();
            m_hoveringInteractable = null;
        }
        if(!m_holdingInteractable) PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.DEFAULT);
    }
    void ClearHoldingInteractable(){
        if(m_holdingInteractable != null){
            m_holdingInteractable.OnRelease(this);
            m_holdingInteractable = null;
        }
        if(!m_hoveringInteractable) PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.DEFAULT);
        else PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.HOVER);
    }
    void InteractWithClickable(){
        if(m_hoveringInteractable.m_isInteractable){
            m_hoveringInteractable.OnClick(this, hoverPos);
            AudioManager.Instance.PlaySoundEffect(playerAudio, m_hoveringInteractable.sfx_clickSound, 1);
        }
        else{
            m_hoveringInteractable.OnFailClick(this);
            AudioManager.Instance.PlaySoundEffect(playerAudio, string.Empty, 0);
        }
    }
    public Vector3 GetCursorWorldPoint(float depth){
        Vector3 mousePoint = PointerScrPos;
        mousePoint.z = depth;
        return mainCam.ScreenToWorldPoint(mousePoint);
    }
    public void HoldInteractable(Basic_Clickable interactable){
        m_holdingInteractable = interactable;
        PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.DRAG);
    }
    public void ReleaseCurrentHolding()=>ClearHoldingInteractable();
    public void CheckControllable(){
        if(PlayerManager.Instance.m_canControl){
            input.ActivateInput();
            this.enabled = true;
        }
        else{
            this.enabled = false;
            if(m_hoveringInteractable) ClearHoveringInteractable();
            if(m_holdingInteractable) ClearHoldingInteractable();
            input.DeactivateInput();
        }
    }
#endregion

#region Player Input
    void OnPointerMove(InputValue value){
        PointerDelta = value.Get<Vector2>();
    }
    void OnPointerPos(InputValue value){
        Vector2 _scrPos = value.Get<Vector2>();
        // _scrPos.x = Mathf.Clamp(_scrPos.x, 0, Screen.width);
        // _scrPos.y = Mathf.Clamp(_scrPos.y, 0, Screen.height);
        PointerScrPos = _scrPos;
    }
    void OnFire(InputValue value){
        if(value.isPressed){
            if(m_holdingInteractable != null) return;
            if(m_hoveringInteractable == null) return;

            InteractWithClickable();
        }
        else{
            ClearHoldingInteractable();
        }
    }
#endregion
}
