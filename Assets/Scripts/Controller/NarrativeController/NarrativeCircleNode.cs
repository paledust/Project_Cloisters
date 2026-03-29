using System;
using UnityEngine;

public class NarrativeCircleNode : MonoBehaviour
{
    private float radius;
    public float m_radius => radius;
    public Vector3 m_position => transform.position;

    public void InitNode(float radius)
    {
        this.radius = radius;
    }
    public void NodeExplode()
    {
        EventHandler.Call_OnNarrativeNodeBreak(this);
    }
}
