using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class NarrativeConnectLine : MonoBehaviour
{
    private LineRenderer lineRenderer;
    private Vector3[] pos;
    private Transform headTrans;
    private Transform tailTrans;
    private float headOffset;
    private float tailOffset;

    public void InitLine(Transform fromTrans, Transform toTrans, float headOffset, float tailOffset, int seg = 3)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = seg;
        headTrans = fromTrans;
        tailTrans = toTrans;
        pos = new Vector3[seg];
        this.headOffset = headOffset;
        this.tailOffset = tailOffset;

        UpdateLine();
    }
    void LateUpdate()
    {
        UpdateLine();
    }
    void UpdateLine()
    {
        Vector3 dir = (tailTrans.position - headTrans.position).normalized;
        for(int i=0; i<pos.Length; i++)
        {
            float t = i / (pos.Length - 1f);
            pos[i] = Vector3.Lerp(headTrans.position + dir * headOffset, tailTrans.position - dir * tailOffset, t);
        }
        lineRenderer.SetPositions(pos);
    }
}
