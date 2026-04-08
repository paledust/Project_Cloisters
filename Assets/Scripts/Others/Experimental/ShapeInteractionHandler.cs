using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class ShapeInteractionHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Transform outlineMaskTrans;
    private Vector3 pos;
    private HashSet<Clickable_ExperimentalShapeDragger> hoveringDraggers;
    private ConnectBody connectBody;

    public event Action onRelease;

    public bool isControlling { get; private set; }

    void Awake()
    {
        connectBody = GetComponent<ConnectBody>();
    }
    void Start()
    {
        hoveringDraggers = new HashSet<Clickable_ExperimentalShapeDragger>();
    }
    public void OnHover(Clickable_ExperimentalShapeDragger dragger)
    {
        if(hoveringDraggers.Count == 0)
        {
            if(connectBody.connectBodies.Count==0)
            {
                pos = transform.InverseTransformPoint(dragger.transform.position);
                outlineMaskTrans.DOKill();
                outlineMaskTrans.DOLocalMove(pos, 0.15f).SetEase(Ease.OutQuad);
                outlineMaskTrans.DOScale(dragger.maskScaleMultiplier, 0.15f).SetEase(Ease.OutQuad);
            }
            else
            {
                outlineMaskTrans.DOKill();
                outlineMaskTrans.DOLocalMove(Vector2.zero, 0.15f).SetEase(Ease.OutQuad);
                outlineMaskTrans.DOScale(10, 0.15f).SetEase(Ease.OutQuad);
            }
        }
        FadeOutline(1, 0.15f);
        hoveringDraggers.Add(dragger);
    }
    public void OnExitHover(Clickable_ExperimentalShapeDragger dragger)
    {
        hoveringDraggers.Remove(dragger);
        if(hoveringDraggers.Count == 0)
        {
            outlineMaskTrans.DOKill();
            outlineMaskTrans.DOLocalMove(Vector3.zero, 0.15f).SetEase(Ease.OutQuad);
            outlineMaskTrans.DOScale(0f, 0.15f).SetEase(Ease.OutQuad);
            FadeOutline(0, 0.15f);
        }
    }
    public void OnControlled()
    {
        isControlling = true;
    }
    public void OnRelease()
    {
        isControlling = false;
        onRelease?.Invoke();
    }
    void FadeOutline(float alpha, float duration)
    {
        outline.DOKill();
        outline.DOFade(alpha, duration);
    }
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.2f);
    }
}