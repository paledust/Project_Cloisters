using System.Collections.Generic;
using UnityEngine;

public struct ShapeConnection
{
    public ConnectTrigger main;
    public ConnectTrigger other;
    public bool Equals(ShapeConnection b)
    {
        return (main == b.main && other == b.other) || (main == b.other && other == b.main);
    }
} 

public class ShapeConnectController : MonoBehaviour
{
    public Dictionary<ShapeConnection, FixedJoint> jointDict;
    void Awake()
    {
        EventHandler.E_OnShapeConnect += OnShapeConnect;
    }
    void OnDestroy()
    {
        EventHandler.E_OnShapeConnect -= OnShapeConnect;
    }
    void OnShapeConnect(ConnectTrigger main, ConnectTrigger other)
    {
        if(jointDict == null) jointDict = new Dictionary<ShapeConnection, FixedJoint>();

        var connection = new ShapeConnection(){main = main, other = other};
        foreach(var key in jointDict.Keys)
        {
            if(key.Equals(connection))
            {
                return;
            }
        }
        if(!jointDict.ContainsKey(connection))
        {
            EventHandler.Call_OnFlashInput();
            
            main.SwitchTrigger(false);
            other.SwitchTrigger(false);
        //Move Both rigid to a propery location
            Vector3 face = (main.transform.up - other.transform.up).normalized;
            float angle = Vector2.SignedAngle(main.transform.up, face);
            main.m_rigid.transform.rotation *= Quaternion.Euler(0,0,angle);
            other.m_rigid.transform.rotation *= Quaternion.Euler(0,0,-angle);
            
            Vector3 mid = (main.transform.position + other.transform.position) * 0.5f;
            Vector3 offset = mid-main.transform.position;
            offset = face*Vector3.Dot(offset, face);
            main.m_rigid.transform.position += 0.8f*offset;
            other.m_rigid.transform.position -= 0.8f*offset;
        //Create Joint
            var joint = main.m_rigid.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = other.m_rigid;

            jointDict.Add(connection, joint);
        }
    }
}
