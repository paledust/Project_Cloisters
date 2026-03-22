using System.Collections.Generic;
using UnityEngine;

public class NarrativeConnectLineController : MonoBehaviour
{
    [SerializeField] private GameObject connectLinePrefab;
    [SerializeField] private Transform connectRoot;

    private List<NarrativeConnectLine> connectLines = new List<NarrativeConnectLine>();
    private HashSet<NarrativeCircleNode> connectedNodeGraph = new HashSet<NarrativeCircleNode>();

    public void BuildConnectLine(NarrativeCircleNode fromCircle, NarrativeCircleNode toCircle)
    {
        GameObject lineObj = Instantiate(connectLinePrefab, connectRoot);
        NarrativeConnectLine line = lineObj.GetComponent<NarrativeConnectLine>();
        line.InitLine(fromCircle.transform, toCircle.transform, fromCircle.m_radius, toCircle.m_radius, 3);
        
        connectLines.Add(line);
        connectedNodeGraph.Add(fromCircle);
        connectedNodeGraph.Add(toCircle);
        fromCircle.ConnectNode(toCircle);
        toCircle.ConnectNode(fromCircle);
    }
    public void BreakConnectedLine(NarrativeCircleNode node)
    {
        node.BreakAllConnectedNode();
    }
    public void CheckConnectLine(Transform transCircle)
    {
        foreach(var line in connectLines)
        {
            line.CheckConnectTrans(transCircle);
        }
    }
}
