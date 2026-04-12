using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.InputSystem;

public class ShapeInteractionHandler : MonoBehaviour
{
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Transform outlineMaskTrans;
    private Vector3 pos;
    private Clickable_ExperimentalShapeDragger currentHoverDragger;
    private ConnectBody connectBody;

    public event Action onRelease;

    public bool isControlling { get; private set; }

    void Awake()
    {
        connectBody = GetComponent<ConnectBody>();
        outline.color = new Color(outline.color.r, outline.color.g, outline.color.b, 0);
    }
    void Update()
    {
        if(currentHoverDragger!=null)
        {
            Vector3 targetPos = currentHoverDragger.transform.position;
            if(!isControlling)
            {
                Vector3 mouseScr = PlayerManager.Instance.GetCursorScreenPos();
                mouseScr.z = 32;
                Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScr);
                targetPos = targetPos + Vector3.ClampMagnitude(mouseWorld - targetPos, .25f);
            }
            outlineMaskTrans.position = Vector3.Lerp(outlineMaskTrans.position, targetPos, Time.deltaTime * 20);
        }
    }
    public void OnHover(Clickable_ExperimentalShapeDragger dragger)
    {
        if(currentHoverDragger == null)
        {
            if(connectBody.connectBodies.Count==0)
            {
                pos = transform.InverseTransformPoint(dragger.transform.position);
                outlineMaskTrans.DOKill();
                // outlineMaskTrans.DOLocalMove(pos, 0.15f).SetEase(Ease.OutQuad);
                outlineMaskTrans.DOScale(dragger.maskScaleMultiplier, 0.15f).SetEase(Ease.OutQuad);
            }
            else
            {
                outlineMaskTrans.DOKill();
                // outlineMaskTrans.DOLocalMove(Vector2.zero, 0.15f).SetEase(Ease.OutQuad);
                outlineMaskTrans.DOScale(10, 0.15f).SetEase(Ease.OutQuad);
            }
            currentHoverDragger = dragger;
        }
        FadeOutline(1, 0.15f);
    }
    public void OnExitHover(Clickable_ExperimentalShapeDragger dragger)
    {
        if(currentHoverDragger == dragger)
        {
            currentHoverDragger = null;
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