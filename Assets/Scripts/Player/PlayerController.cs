using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

[Header("Audio")]
    [SerializeField] private AudioSource playerAudio;

    public Basic_Clickable m_hoveringInteractable{get; private set;} //The hovering interactable.
    public Basic_Clickable m_holdingInteractable{get; private set;} //Currently holding interactable.
    public Vector2 PointerScrPos{get; private set;}

    private Vector3 hoverPos;
    private Camera mainCam;

    void Start()
    {
        mainCam = Camera.main;    
    }

    // Update is called once per frame
    void Update()
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
                    if(!m_holdingInteractable) PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.INTERACT);
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
        else PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.INTERACT);
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
    public void HoldInteractable(Basic_Clickable interactable){
        m_holdingInteractable = interactable;
        PlayerManager.Instance.UpdateCursorState(CURSOR_STATE.DRAG);
    }
#endregion

#region Player Input
    void OnPointerPos(InputValue value){
        Vector2 _scrPos = value.Get<Vector2>();
        _scrPos.x = Mathf.Clamp(_scrPos.x, 0, Screen.width);
        _scrPos.y = Mathf.Clamp(_scrPos.y, 0, Screen.height);
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