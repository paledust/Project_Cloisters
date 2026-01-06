using System.Collections.Generic;
using UnityEngine;

public class NarrativeConnectLineController : MonoBehaviour
{
    [SerializeField] private GameObject connectLinePrefab;
    [SerializeField] private Transform connectRoot;
    private List<NarrativeConnectLine> connectLines = new List<NarrativeConnectLine>();
    public void BuildConnectLine(Clickable_Circle fromCircle, Clickable_Circle toCircle)
    {
        GameObject lineObj = Instantiate(connectLinePrefab, connectRoot);
        NarrativeConnectLine line = lineObj.GetComponent<NarrativeConnectLine>();
        line.InitLine(fromCircle.transform, toCircle.transform, fromCircle.radius, toCircle.radius, 3);
        
        connectLines.Add(line);
    }
    public void CheckConnectLine(Transform transCircle)
    {
        foreach(var line in connectLines)
        {
            line.CheckConnectTrans(transCircle);
        }
    }
}
