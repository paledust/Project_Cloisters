using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConnectBody : MonoBehaviour
{
    [SerializeField] private float sphereRadius;
    [SerializeField] private bool isSpherical = false;
    [SerializeField] private ShapeColorChanger shapeColorChanger;
[Header("Vertices")]
    [SerializeField] private Transform[] points;
    private ConnectTrigger[] connectTriggers;
    private ConnectTrigger pendingTrigger; //the ideal connected trigger
    private ConnectTrigger occupiedTrigger; //the trigger attached that connected to ideal trigger
    public HashSet<ConnectBody> connectBodies = new HashSet<ConnectBody>();
    private Clickable_Moveable clickable_Moveable;

    public Rigidbody m_rigid{get; private set;}
    public bool m_isSpherical=>isSpherical;
    public float m_sphereRadius=>isSpherical?sphereRadius:0;
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
    public void BlinkShape(Color blinkColor1, Color blinkColor2)=>shapeColorChanger.BlinkColor(blinkColor1, blinkColor2);
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
            if(!m_isSpherical)
            {
                Quaternion targetRot;
                if(pendingTrigger.m_connectBody.m_isSpherical)
                {
                    float angle = Vector2.SignedAngle(occupiedTrigger.normal, pendingTrigger.transform.position - transform.position);
                    targetRot = Quaternion.Euler(0,0,angle) * m_rigid.rotation;
                }
                else
                {
                    targetRot = offsetRotToOther * pendingTrigger.m_connectBody.m_rigid.rotation;
                }
                Quaternion idealRot = Quaternion.Lerp(m_rigid.rotation, targetRot, Time.fixedDeltaTime * (pendingTrigger.m_connectBody.m_isSpherical?10:5));
                m_rigid.MoveRotation(idealRot);
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
            SwapConnection(null, null);
        }
        else
        {
            foreach(ConnectTrigger connectTrigger in connectTriggers)
            {
                connectTrigger.ResetTrigger();
            }
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
            if(!m_isSpherical)
            {
                float rotation;
                if(!otherTrigger.m_connectBody.m_isSpherical) 
                {
                    rotation = Vector2.SignedAngle(occupiedTrigger.normal, -pendingTrigger.normal);
                    Quaternion idealRot = Quaternion.Euler(0,0,rotation) * m_rigid.rotation;
                    Quaternion otherBodyRot = pendingTrigger.m_connectBody.m_rigid.rotation;
                    offsetRotToOther = idealRot * Quaternion.Inverse(otherBodyRot);
                }
                else
                {
                    rotation = Vector2.SignedAngle(occupiedTrigger.normal, otherTrigger.transform.position - transform.position);
                    Quaternion idealRot = Quaternion.Euler(0,0,rotation) * m_rigid.rotation;
                    offsetRotToOther = idealRot;
                }
            }

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
    public Vector3 GetCenter()
    {
        Vector3 center = m_rigid.centerOfMass;
        center.z = 0;
        return transform.TransformPoint(center);
    }
    public List<ConnectTrigger> GetAllFreeTrigger()
    {
        List<ConnectTrigger> trigger = new List<ConnectTrigger>(connectTriggers);
        return trigger.FindAll(x=>!x.m_isLocked);
    }
    public Vector2[] GetAllPoints()
    {
        var vertices = new Vector2[points.Length];
        for(int i=0; i<points.Length; i++)
        {
            vertices[i] = points[i].position;
        }
        return vertices;
    }
    void OnDrawGizmos()
    {
        if(isSpherical)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, sphereRadius);
        }
    }
}