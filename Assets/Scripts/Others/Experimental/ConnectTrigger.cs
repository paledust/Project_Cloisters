using System.Collections.Generic;
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
    [SerializeField] private ConnectTriggerState connectTriggerState = ConnectTriggerState.Pending;
    [SerializeField] private Basic_Clickable selfClickable;
    [SerializeField] private SpriteRenderer alignMask;
    private Collider m_collider;
    private ConnectTrigger catchingTrigger;
    private List<ConnectTrigger> pendingTriggers;

    public Vector3 normal => transform.up.normalized;
    public ConnectBody m_connectBody{get; private set;}

    private const float MIN_CONNECT_DOT = 0.92f;

    void Reset()
    {
        selfClickable = GetComponentInParent<Basic_Clickable>();
    }
    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_connectBody = selfClickable.GetComponent<ConnectBody>();
        pendingTriggers = new List<ConnectTrigger>();
    }
    public ConnectTrigger UpdateTriggerDetection(out float idealDot)
    {
        idealDot = 0;
        switch(connectTriggerState)
        {
            case ConnectTriggerState.Pending:
                if(pendingTriggers.Count>0)
                {
                    ChangeState(ConnectTriggerState.Detecting);
                    return null;
                }
                return null;
            case ConnectTriggerState.Detecting:
                //Check the best target
                if(pendingTriggers.Count==0)
                {
                    ChangeState(ConnectTriggerState.Pending);
                    return null;
                }
                int bestIndex = -1;
                for(int i=pendingTriggers.Count-1; i>=0; i--)
                {
                    float dot = Vector2.Dot(normal, -pendingTriggers[i].normal);
                    if(dot>=MIN_CONNECT_DOT && dot>idealDot)
                    {
                        idealDot = dot;
                        bestIndex = pendingTriggers.Count - 1;
                    }
                }
                if(bestIndex>=0)
                {
                    return pendingTriggers[bestIndex];
                }
                return null;
            case ConnectTriggerState.Catching:
                Vector2 balanceDir = (catchingTrigger.normal - normal).normalized;
                idealDot = Vector2.Dot(normal, -catchingTrigger.normal);
                float dist = Vector2.Dot(balanceDir, transform.position-catchingTrigger.transform.position);
                if(idealDot>=MIN_CONNECT_DOT && Mathf.Abs(dist)<=0.5f)
                {
                    return catchingTrigger;
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
        if(otherTrigger!=null && otherTrigger.m_connectBody!=m_connectBody)
        {
            if(!pendingTriggers.Contains(otherTrigger))
            {
                pendingTriggers.Add(otherTrigger);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        var otherTrigger = other.GetComponent<ConnectTrigger>();
        if(otherTrigger != null)
        {
            if(pendingTriggers.Contains(otherTrigger))
            {
                pendingTriggers.Remove(otherTrigger);
            }
        }
    }
    public void SwitchTrigger(bool isEnable)
    {
        m_collider.enabled = isEnable;
    }
    public void OnConnectionBuild()
    {
        ChangeState(ConnectTriggerState.Locking);
        m_collider.enabled = false;
    }
    public void OnConnectionBreak()
    {
        m_collider.enabled = true;
        ChangeState(ConnectTriggerState.Pending);
    }
    public void OnConnectionCatch(ConnectTrigger alignTrigger)
    {
        catchingTrigger = alignTrigger;
        catchingTrigger.FadeMask(1f);
        FadeMask(1f);
        ChangeState(ConnectTriggerState.Catching);
    }
    public void OnLostConnectionCatching()
    {
        FadeMask(0f);
        catchingTrigger.FadeMask(0f);
        catchingTrigger = null;
        ChangeState(ConnectTriggerState.Pending);
    }
    public void FadeMask(float alpha)
    {
        alignMask.DOKill();
        alignMask.DOFade(alpha, 0.2f);
    }
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        switch(connectTriggerState)
        {
            case ConnectTriggerState.Pending:
                Gizmos.color = Color.green;
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
        
        Gizmos.DrawSphere(Vector3.zero, 0.2f);
    }
}
