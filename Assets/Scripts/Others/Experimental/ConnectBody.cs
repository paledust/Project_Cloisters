using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;

public class ConnectBody : MonoBehaviour
{
    private ConnectTrigger[] connectTriggers;
    private ConnectTrigger pendingTrigger; //the ideal connected trigger
    private ConnectTrigger occupiedTrigger; //the trigger attached that connected to ideal trigger
    public HashSet<ConnectBody> connectBodies = new HashSet<ConnectBody>();
    private Clickable_Moveable clickable_Moveable;

    public Rigidbody m_rigid{get; private set;}
    private bool hasIdealConnection => occupiedTrigger != null;
    private Quaternion offsetRotToOther;

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
            if(!hasIdealConnection)
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
            else
            {
                ConnectTrigger otherTrigger = occupiedTrigger.UpdateTriggerDetection(out float dot);
                if(otherTrigger == null)
                {
                    occupiedTrigger.OnLostConnectionCatching();
                    SwapConnection(null, null);
                }
            }
        }
    }
    void FixedUpdate()
    {
        if(clickable_Moveable.isControlling && hasIdealConnection)
        {
            Quaternion idealRot = Quaternion.Lerp(m_rigid.rotation, offsetRotToOther * pendingTrigger.m_connectBody.m_rigid.rotation, Time.fixedDeltaTime * 5);
            m_rigid.MoveRotation(idealRot);
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
            SwapConnection(null, null);
        }
    }
    void SwapConnection(ConnectTrigger selfTrigger, ConnectTrigger otherTrigger)
    {
        pendingTrigger = otherTrigger;
        occupiedTrigger = selfTrigger;
    //if Catch any good trigger, align the control
        if(hasIdealConnection)
        {
            occupiedTrigger.OnConnectionCatch(pendingTrigger);
        //Find the align rotation
            float rotation = Vector2.SignedAngle(occupiedTrigger.normal, -pendingTrigger.normal);
            Quaternion idealRot = Quaternion.Euler(0,0,rotation) * m_rigid.rotation;
            Quaternion otherBodyRot = pendingTrigger.m_connectBody.m_rigid.rotation;
            offsetRotToOther = idealRot * Quaternion.Inverse(otherBodyRot);

            m_rigid.freezeRotation = true;
        }
        else
        {
            m_rigid.constraints = RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
        }
    }
    public void BuildConnection(ConnectBody body)=>connectBodies.Add(body);
    public void BreakConnection(ConnectBody body)=>connectBodies.Remove(body);
    public bool IsConnectedToBody(ConnectBody body)=>connectBodies.Contains(body);
}