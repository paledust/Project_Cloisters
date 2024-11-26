using UnityEngine;

public abstract class Basic_Clickable : MonoBehaviour
{
[Header("Clickable Basic")]
    public string sfx_clickSound = string.Empty;
    [SerializeField] protected Collider hitbox;
    [SerializeField] protected bool isFrozen = false; //If not available, player can still click on object but will show stop sign
    
    public bool m_isInteractable{get{return gameObject.activeInHierarchy && !isFrozen && hitbox.enabled;}}
    public Collider m_hitbox{get{return hitbox;}}
    
    void Reset()=>hitbox = GetComponent<Collider>();

#region Interaction Function
    public virtual void OnHover(PlayerController player){}
    public virtual void OnExitHover(){}
    public virtual void OnClick(PlayerController player, Vector3 hitPos){}
    public virtual void OnRelease(PlayerController player){}
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
