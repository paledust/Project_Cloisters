using System.Collections.Generic;
using UnityEngine;

public class NarrativeConnectLineController : MonoBehaviour
{
    [SerializeField] private GameObject connectLinePrefab;
    [SerializeField] private Transform connectRoot;

    private Dictionary<NarrativeCircleNode, HashSet<NarrativeConnectLine>> nodeConnectionGraph = new Dictionary<NarrativeCircleNode, HashSet<NarrativeConnectLine>>();
    private Dictionary<NarrativeCircleNode, HashSet<NarrativeCircleNode>> nodeGraph = new Dictionary<NarrativeCircleNode, HashSet<NarrativeCircleNode>>();

    void Awake()
    {
        EventHandler.E_OnNarrativeNodeBreak += NodeBreakHandler;
    }
    void OnDestroy()
    {
        EventHandler.E_OnNarrativeNodeBreak -= NodeBreakHandler;
    }
    void NodeBreakHandler(NarrativeCircleNode node)
    {
        BreakLineForNode(node);
    }
    public void BuildConnectLine(NarrativeCircleNode fromCircle, NarrativeCircleNode toCircle)
    {
        if(nodeGraph.ContainsKey(fromCircle) && nodeGraph[fromCircle].Contains(toCircle))
        {
            return;
        }

        GameObject lineObj = Instantiate(connectLinePrefab, connectRoot);
        NarrativeConnectLine line = lineObj.GetComponent<NarrativeConnectLine>();
        line.InitLine(fromCircle, toCircle, fromCircle.m_radius, toCircle.m_radius, 3);
        
        //Build Graph
        AddNodeToGraph(fromCircle, toCircle, line);
        AddNodeToGraph(toCircle, fromCircle, line);
    }
    public void BreakLineForNode(NarrativeCircleNode circle)
    {
        if(!nodeConnectionGraph.ContainsKey(circle))
        {
            return;
        }
        //Break Lines connected to this Node.
        var lines = nodeConnectionGraph[circle];
        foreach(var line in lines)
        {
            if(line.IsDisappearing)
                continue;
            var anotherNode = line.GetAnotherNode(circle);
            nodeConnectionGraph[anotherNode].Remove(line);
            line.BreakLine();
        }
        nodeConnectionGraph.Remove(circle);
    }
    public void AddNodeToGraph(NarrativeCircleNode fromNode, NarrativeCircleNode toNode, NarrativeConnectLine line)
    {
        if(nodeGraph.ContainsKey(fromNode))
        {
            nodeGraph[fromNode].Add(toNode);
        }
        else
        {
            nodeGraph[fromNode] = new HashSet<NarrativeCircleNode>() { toNode };
        }

        if(nodeConnectionGraph.ContainsKey(fromNode))
        {
            nodeConnectionGraph[fromNode].Add(line);
        }
        else
        {
            nodeConnectionGraph[fromNode] = new HashSet<NarrativeConnectLine>() { line };
        }
    }
}
