using System.Collections.Generic;
using UnityEngine;

public class NarrativeCircleNode : MonoBehaviour
{
    private HashSet<NarrativeCircleNode> connectedNodes = new HashSet<NarrativeCircleNode>();
    private float radius;
    public float m_radius => radius;
    public void InitNode(float radius)
    {
        this.radius = radius;
    }
    public void ConnectNode(NarrativeCircleNode node)
    {
        if (!connectedNodes.Contains(node))
        {
            connectedNodes.Add(node);
        }
    }
    public void RemoveNode(NarrativeCircleNode node)
    {
        if (connectedNodes.Contains(node))
        {
            connectedNodes.Remove(node);
        }
    }
    public void BreakAllConnectedNode()
    {
        foreach(var node in connectedNodes)
        {
            node.RemoveNode(this);
        }
        connectedNodes.Clear();
    }
}
