using System.Collections;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NarrativeConnectLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3[] pos;
    private NarrativeCircleNode headNode;
    private NarrativeCircleNode tailNode;
    private Joint joint;
    private float headOffset;
    private float tailOffset;
    private bool isDisappearing = false;
    public bool IsDisappearing => isDisappearing;

    public void InitLine(NarrativeCircleNode fromNode, NarrativeCircleNode toNode, float headOffset, float tailOffset, int seg = 3)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = seg;
        StartCoroutine(coroutineFormJoint(1f, fromNode.GetComponent<Rigidbody>(), toNode.GetComponent<Rigidbody>()));

        headNode = fromNode;
        tailNode = toNode;
        pos = new Vector3[seg];
        this.headOffset = headOffset;
        this.tailOffset = tailOffset;

        UpdateLine(headNode.m_position, tailNode.m_position);
    }
    void LateUpdate()
    {
        UpdateLine(headNode.m_position, tailNode.m_position);
    }
    void UpdateLine(Vector3 headPos, Vector3 tailPos)
    {
        Vector3 dir = (tailPos - headPos).normalized;
        for(int i=0; i<pos.Length; i++)
        {
            float t = i / (pos.Length - 1f);
            pos[i] = Vector3.Lerp(headPos + dir * headOffset, tailPos - dir * tailOffset, t);
        }
        lineRenderer.SetPositions(pos);
    }
    public void BreakLine()
    {
        isDisappearing = true;
        StartCoroutine(coroutineDisappear());
    }
    public NarrativeCircleNode GetAnotherNode(NarrativeCircleNode node)
    {
        if(node == headNode)
        {
            return tailNode;
        }
        else if(node == tailNode)
        {
            return headNode;
        }
        return null;
    }
    IEnumerator coroutineDisappear()
    {
        this.enabled = false;
        Vector3 initHeadPos = headNode.m_position;
        Vector3 initTailPos = tailNode.m_position;
        Vector3 midPos = (initHeadPos + initTailPos) * 0.5f;
        yield return new WaitForLoop(0.25f, (t) =>
        {
            Vector3 headPos = Vector3.Lerp(initHeadPos, midPos, EasingFunc.Easing.QuadEaseIn(t));
            Vector3 tailPos = Vector3.Lerp(initTailPos, midPos, EasingFunc.Easing.QuadEaseIn(t));
            UpdateLine(headPos, tailPos);
        });
        UpdateLine(midPos, midPos);
        yield return null;
        gameObject.SetActive(false);
    }
    IEnumerator coroutineFormJoint(float delay, Rigidbody fromNode, Rigidbody toNode)
    {
        yield return new WaitForSeconds(delay);
        if(fromNode == null || toNode == null)
        {
            yield break;
        }
        joint = PhysicDragManager.GetNewBodyConnector(fromNode.GetComponent<Rigidbody>(), toNode.GetComponent<Rigidbody>(), 10f, 2f);
    }
}