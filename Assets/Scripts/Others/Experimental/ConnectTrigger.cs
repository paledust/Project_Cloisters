using UnityEngine;
using DG.Tweening;

public class ConnectTrigger : MonoBehaviour
{
    public enum ConnectTriggerState
    {
        Pending, //Not found Any Trigger
        Detecting, //Found targets, and validate
        Catching, //Found the best target, and keep focusing 
        Locking, //Connected to target
    }
    [SerializeField] private bool isSunk = false;
    [SerializeField] private ConnectTriggerState connectTriggerState = ConnectTriggerState.Pending;
    [SerializeField] private SpriteRenderer alignMask;
    private Collider m_collider;
    private ShapeInteractionHandler interactionHandler;
    private ConnectTrigger pendingTrigger;

    public bool m_isLocked => connectTriggerState == ConnectTriggerState.Locking;
    public bool m_isSunk => isSunk;
    public Vector3 normal => transform.up.normalized;
    public ConnectBody m_connectBody{get; private set;}

    private const float MAX_BREAK_DOT = 0.85f;
    private const float MAX_BREAK_DIST = 1.2f;

    void Start()
    {
        m_collider = GetComponent<Collider>();
        interactionHandler = GetComponentInParent<ShapeInteractionHandler>();
        m_connectBody = interactionHandler.GetComponent<ConnectBody>();
        Color defaultColor = alignMask.color;
        defaultColor.a = 0;
        alignMask.color = defaultColor;
    }
    public ConnectTrigger UpdateTriggerDetection(out float idealDot)
    {
        idealDot = 0;
        switch(connectTriggerState)
        {
            case ConnectTriggerState.Pending:
                if(pendingTrigger!=null)
                {
                    ChangeState(ConnectTriggerState.Detecting);
                    return null;
                }
                return null;
            case ConnectTriggerState.Detecting:
                if(pendingTrigger == null)
                {
                    ChangeState(ConnectTriggerState.Pending);
                    return null;
                }

                idealDot = Vector2.Dot(pendingTrigger.normal, -normal);
                return pendingTrigger;
            case ConnectTriggerState.Catching:
                if(pendingTrigger == null)
                {
                    ChangeState(ConnectTriggerState.Pending);
                    return null;
                }

                Vector2 balanceDir;
                balanceDir = (pendingTrigger.normal - normal).normalized;
                idealDot = Vector2.Dot(normal, -pendingTrigger.normal);
                float dist; 
                Vector2 diff = transform.position-pendingTrigger.transform.position;
                dist = Vector2.Dot(balanceDir, diff);

                if(idealDot>=MAX_BREAK_DOT && Mathf.Abs(dist)<=MAX_BREAK_DIST)
                {
                    return pendingTrigger;
                }
                return null;
            default:
                return null;
        }
    }
    void ChangeState(ConnectTriggerState nextState)
    {
        connectTriggerState = nextState;
    }
    void OnTriggerEnter(Collider other)
    {
        var otherTrigger = other.GetComponent<ConnectTrigger>();
        if(otherTrigger!=null && otherTrigger.m_connectBody!=m_connectBody && !m_connectBody.IsConnectedToBody(otherTrigger.m_connectBody))
        {
            if(m_connectBody.m_iscontrolling)
            {
                (Vector3 pos, Quaternion rot) = ShapeConnectController.GetConnectTransform(this, otherTrigger, 0.005f);
                if(ShapeConnectValidator.ValidateBodyAt(m_connectBody, otherTrigger.m_connectBody, pos, rot))
                if(this.isSunk != otherTrigger.isSunk)
                    pendingTrigger = otherTrigger;
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        var otherTrigger = other.GetComponent<ConnectTrigger>();
        if(otherTrigger != null)
        {
            if(pendingTrigger == otherTrigger)
            {
                pendingTrigger.FadeMask(0f);
                pendingTrigger = null;
            }
        }
    }
    public void ResetTrigger()
    {
        if(!m_isLocked)
        {
            m_collider.enabled = false;
            pendingTrigger = null;
            m_collider.enabled = true;
        }
    }
    public void OnConnectionBuild()
    {
        ChangeState(ConnectTriggerState.Locking);
        FadeMask(0, 0.5f);
        m_collider.enabled = false;
    }
    public void OnConnectionBreak()
    {
        m_collider.enabled = true;
        ChangeState(ConnectTriggerState.Pending);
    }
    public void OnConnectionCatch(ConnectTrigger alignTrigger)
    {
        alignTrigger.FadeMask(1f);
        FadeMask(1f);
        ChangeState(ConnectTriggerState.Catching);
    }
    public void OnLostConnectionCatching()
    {
        FadeMask(0f);
        ChangeState(ConnectTriggerState.Pending);
    }
    void FadeMask(float alpha, float delay = 0f)
    {
        alignMask.DOKill();
        alignMask.DOFade(alpha, 0.2f).SetDelay(delay);
    }
    void OnDrawGizmos()
    {
        switch(connectTriggerState)
        {
            case ConnectTriggerState.Pending:
                Gizmos.color = isSunk?Color.black:Color.white;
                break;
            case ConnectTriggerState.Detecting:
                Gizmos.color = Color.yellow;
                break;
            case ConnectTriggerState.Catching:
                Gizmos.color = Color.blue;
                break;
            case ConnectTriggerState.Locking:
                Gizmos.color = Color.red;
                break;
        }
        
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
}
