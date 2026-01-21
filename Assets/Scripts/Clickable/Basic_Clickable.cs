using System;
using UnityEngine;

public abstract class Basic_Clickable : MonoBehaviour
{
[Header("Clickable Basic")]
    public string sfx_clickSound = "group_click";
    [SerializeField] protected Collider hitbox;
    [SerializeField] protected bool isFrozen = false; //If not available, player can still click on object but will show stop sign
    
    public bool m_isInteractable{get{return gameObject.activeInHierarchy && !isFrozen && hitbox.enabled;}}
    public Collider m_hitbox{get{return hitbox;}}
    public bool isControlling{get; private set;}

    public event Action onClick;
    public event Action onRelease;
    public event Action<PlayerController> onHover;
    public event Action onExitHover;
    
    void Reset()=>hitbox = GetComponent<Collider>();

#region Interaction Function
    public virtual void OnHover(PlayerController player){onHover?.Invoke(player);}
    public virtual void OnExitHover(){onExitHover?.Invoke();}
    public virtual void OnClick(PlayerController player, Vector3 hitPos){
        isControlling = true;
        onClick?.Invoke();
    }
    public virtual void OnRelease(PlayerController player){
        isControlling = false;
        onRelease?.Invoke();
    }
    public virtual void OnFailClick(PlayerController player){}
    public virtual void ControllingUpdate(PlayerController player){}
    protected virtual void OnBecomeInteractable(){}
    protected virtual void OnBecomeUninteractable(){}
#endregion

#region Interaction Activation
    public void FreezeInteraction(){
        if(m_isInteractable) OnBecomeUninteractable();
        isFrozen = true;
    }
    public void UnfreezeInteraction(){
        if(gameObject.activeInHierarchy && m_hitbox.enabled) OnBecomeInteractable();
        isFrozen = false;
    }
    public void DisableHitbox(){
        if(m_isInteractable) OnBecomeUninteractable();
        m_hitbox.enabled = false;
    }
    public void EnableHitbox(){
        if(gameObject.activeInHierarchy && isFrozen) OnBecomeInteractable();
        m_hitbox.enabled = true;
    }
    public void EnableRaycast()=>gameObject.layer = Service.InteractableLayer;
    public void DisableRaycast()=>gameObject.layer = Service.IgnoreRaycastLayer;
#endregion
}
