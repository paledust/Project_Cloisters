using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectTrigger : MonoBehaviour
{
    [SerializeField] private bool isRound = false;
    [SerializeField] private Basic_Clickable selfClickable;
    private Collider m_collider;

    public Rigidbody m_rigid{get; private set;}

    private const float MIN_CONNECT_DOT = 0.95f;

    void Reset()
    {
        selfClickable = GetComponentInParent<Basic_Clickable>();
    }
    void Start()
    {
        m_collider = GetComponent<Collider>();
        m_rigid = selfClickable.GetComponent<Rigidbody>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(!selfClickable.isControlling) return;

        var otherTrigger = other.GetComponent<ConnectTrigger>();
        if(otherTrigger == null) return;
        
        if(Vector3.Dot(transform.up.normalized, -otherTrigger.transform.up.normalized)>=MIN_CONNECT_DOT)
        {
            EventHandler.Call_OnShapeConnect(this, otherTrigger);
        }
    }
    public void SwitchTrigger(bool isEnable)
    {
        m_collider.enabled = isEnable;
    }
}
