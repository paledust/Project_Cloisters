using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectBody : MonoBehaviour
{
    [ShowOnly, SerializeField] private ConnectTrigger[] connectTriggers;
    
    [ShowOnly, SerializeField] private ConnectTrigger pendingTrigger;
    [ShowOnly, SerializeField] private ConnectTrigger occupiedTrigger;
    public HashSet<ConnectBody> connectBodies = new HashSet<ConnectBody>();
    private Clickable_Moveable clickable_Moveable;

    public Rigidbody m_rigid{get; private set;}

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
        clickable_Moveable = GetComponent<Clickable_Moveable>();
        connectTriggers = GetComponentsInChildren<ConnectTrigger>();
    }
    void Start()
    {
        clickable_Moveable.onClick += OnControlBody;
        clickable_Moveable.onRelease += OnReleaseBody;
    }
    void OnDestroy()
    {
        clickable_Moveable.onClick += OnControlBody;
        clickable_Moveable.onRelease -= OnReleaseBody;
    }
    void OnControlBody(){}
    void Update()
    {
        if(clickable_Moveable.isControlling)
        {
            float bestDot;
            bestDot = 0;
            foreach(ConnectTrigger connectTrigger in connectTriggers)
            {
                ConnectTrigger otherTrigger = connectTrigger.UpdateTriggerDetection(out float dot);
                if(dot>bestDot)
                {
                    bestDot = dot;
                    SwapConnection(connectTrigger, otherTrigger);
                }
            }
            if(bestDot==0)
            {
                SwapConnection(null, null);
            }
        }
    }
    void OnReleaseBody()
    {
        if(pendingTrigger!=null && occupiedTrigger!=null)
        {
            if(!IsConnectedToBody(pendingTrigger.m_connectBody))
            {
                EventHandler.Call_OnShapeConnect(occupiedTrigger, pendingTrigger);
            }
        }
    }
    void SwapConnection(ConnectTrigger selfTrigger, ConnectTrigger otherTrigger)
    {
        pendingTrigger = otherTrigger;
        occupiedTrigger = selfTrigger;
    }
    public void BuildConnection(ConnectBody body)=>connectBodies.Add(body);
    public void BreakConnection(ConnectBody body)=>connectBodies.Remove(body);
    public bool IsConnectedToBody(ConnectBody body)=>connectBodies.Contains(body);
}