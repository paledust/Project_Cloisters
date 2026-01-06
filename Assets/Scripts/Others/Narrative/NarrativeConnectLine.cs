using System.Collections;
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
    private bool isDisappearing = false;

    public void InitLine(Transform fromTrans, Transform toTrans, float headOffset, float tailOffset, int seg = 3)
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.positionCount = seg;
        headTrans = fromTrans;
        tailTrans = toTrans;
        pos = new Vector3[seg];
        this.headOffset = headOffset;
        this.tailOffset = tailOffset;

        UpdateLine(headTrans.position, tailTrans.position);
    }
    void LateUpdate()
    {
        UpdateLine(headTrans.position, tailTrans.position);
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
    public void CheckConnectTrans(Transform targetTrans)
    {
        if(isDisappearing)
            return;
        if(targetTrans == headTrans || targetTrans == tailTrans)
        {
            isDisappearing = true;
            StartCoroutine(coroutineDisappear());
        }
    }
    IEnumerator coroutineDisappear()
    {
        this.enabled = false;
        Vector3 initHeadPos = headTrans.position;
        Vector3 initTailPos = tailTrans.position;
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
}