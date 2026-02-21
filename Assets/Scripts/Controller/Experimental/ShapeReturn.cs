using System.Collections.Generic;
using UnityEngine;

public class ShapeReturn : MonoBehaviour
{
    [SerializeField] private Vector2 force;
    
    void OnTriggerStay(Collider collider)
    {
        var rigid = collider.attachedRigidbody;
        if (rigid != null)
        {
            rigid.AddForce(transform.rotation * force, ForceMode.Acceleration);
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        DebugExtension.DrawArrow(Vector3.zero, force, Color.blue);
    }
}