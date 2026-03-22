using System.Collections.Generic;
using UnityEngine;

public class NarrativeConnectLineController : MonoBehaviour
{
    [SerializeField] private GameObject connectLinePrefab;
    [SerializeField] private Transform connectRoot;

    private Dictionary<NarrativeCircleNode, HashSet<NarrativeConnectLine>> nodeGraph = new Dictionary<NarrativeCircleNode, HashSet<NarrativeConnectLine>>();

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
        GameObject lineObj = Instantiate(connectLinePrefab, connectRoot);
        NarrativeConnectLine line = lineObj.GetComponent<NarrativeConnectLine>();
        line.InitLine(fromCircle, toCircle, fromCircle.m_radius, toCircle.m_radius, 3);
        
        //Build Graph
        FormConnection(toCircle, fromCircle, line);
        FormConnection(fromCircle, toCircle , line);
    }
    public void BreakLineForNode(NarrativeCircleNode circle)
    {
        if(!nodeGraph.ContainsKey(circle))
        {
            return;
        }
        //Break Lines connected to this Node.
        var lines = nodeGraph[circle];
        foreach(var line in lines)
        {
            if(line.IsDisappearing)
                continue;
            var anotherNode = line.GetAnotherNode(circle);
            nodeGraph[anotherNode].Remove(line);
            line.BreakLine();
        }
        nodeGraph.Remove(circle);
    }
    void FormConnection(NarrativeCircleNode fromCircle, NarrativeCircleNode toCircle, NarrativeConnectLine line)
    {
        //Build Graph, Add Line into Graph.
        if(nodeGraph.ContainsKey(fromCircle))
        {
            nodeGraph[fromCircle].Add(line);
        }
        else
        {
            nodeGraph[fromCircle] = new HashSet<NarrativeConnectLine>() { line };
        }
    }
}
