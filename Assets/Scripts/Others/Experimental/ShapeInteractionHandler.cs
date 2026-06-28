using System;
using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;

public class ShapeInteractionHandler : MonoBehaviour
{
    [SerializeField] private Transform renderRoot;
    [SerializeField] private SpriteRenderer outline;
    [SerializeField] private Transform outlineMaskTrans;
    [SerializeField] private AudioData_SO sfxOnCenter;
    [SerializeField] private AudioData_SO sfxOnCorner;
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
            Vector3 mouseScr = PlayerManager.Instance.GetCursorScreenPos();
            mouseScr.z = 32;
            Vector3 mouseWorld = Camera.main.ScreenToWorldPoint(mouseScr);
            Vector3 targetPos = currentHoverDragger.transform.position;

            if(!isControlling)
            {
                if(!currentHoverDragger.IsCenter)
                {
                    targetPos = targetPos + Vector3.ClampMagnitude(mouseWorld - currentHoverDragger.transform.position, .25f);
                }
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
                outlineMaskTrans.DOKill();
                outlineMaskTrans.DOScale(dragger.maskScaleMultiplier, 0.15f).SetEase(Ease.OutQuad);
            }
            else
            {
                outlineMaskTrans.DOKill();
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
    public void OnControlled(bool isCenter)
    {
        isControlling = true;
        renderRoot.localRotation = Quaternion.identity;
        renderRoot.DOKill();
        renderRoot.DOPunchRotation(Vector3.forward * (isCenter?8:4), 0.25f, isCenter?25:17);
        AudioManager.Instance.PlaySFX(isCenter?sfxOnCenter.AudioKey:sfxOnCorner.AudioKey, 1);
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
        var rigid = GetComponent<Rigidbody>();
        Gizmos.DrawSphere(rigid.centerOfMass, 0.2f);
    }
}