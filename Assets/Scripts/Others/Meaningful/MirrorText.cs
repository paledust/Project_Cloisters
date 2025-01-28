using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MirrorText : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] public Color foundColor;
    [SerializeField] private char textChar;
    [SerializeField] private PerRendererStylizedShade fontColor;
    [SerializeField] private float focusRadius = 0.5f;
    [SerializeField] private float searchRadius = 2f;
    [SerializeField] private Collider hitbox;
    [SerializeField] private Transform mirrorCenter;
    [SerializeField] private ParticleSystem p_spot;
[Header("Motion")]
    [SerializeField] private AngleMotion angleMotion;
    [SerializeField] private FloatingMotion floatMotion;

    private Color originColor;
    private bool isFocus = false;
    private bool isRevealed  = false;
    private float focusFactor = 0;
    public float m_focusFactor => isFocus?0:focusFactor;

    public char TextChar => textChar;

    void Awake()
    {
        originColor = fontColor.Tint;
    }
    public void CopyText(MirrorText mirrorText)
    {
        isFocus = mirrorText.isFocus;
        isRevealed = mirrorText.isRevealed;
        focusFactor = mirrorText.focusFactor;
        textChar = mirrorText.textChar;
        hitbox.enabled = mirrorText.hitbox.enabled;
        fontColor.Tint = mirrorText.fontColor.Tint;
        DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, Color.white, 2f);
    }
    public bool TryFocusMirrorText(Vector3 hitPoint)
    {
        Vector3 diff = hitPoint - transform.position;
        Vector3 mirrorDir = (mirrorCenter.position - transform.position).normalized;
        Vector3 t = diff - mirrorDir * Vector3.Dot(mirrorDir, diff);

        focusFactor = Mathf.Clamp01((t.magnitude-focusRadius)/(searchRadius-focusRadius));

        if(t.sqrMagnitude<focusRadius*focusRadius)
        {
            if(!isFocus)
            {
                isFocus = true;
                transform.DOKill();
                transform.DOScale(0.18f, 1f)
                .SetEase(Ease.InQuad);

                DOTween.Kill(this);
                DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, foundColor, 1f)
                .SetEase(Ease.InQuad)
                .SetId(this)
                .OnComplete(()=>{
                    hitbox.enabled = false;
                    isRevealed = true;
                    isFocus = false;
                    p_spot.Play();

                //Reposition TextMesh
                    Vector3 dir = GetReflectDir(mirrorCenter.position-transform.position);
                    Vector3 up  = GetReflectDir(transform.up);
                    angleMotion.enabled = false;
                    floatMotion.enabled = false;

                    Vector3 newPos = mirrorCenter.position - dir;
                    newPos = Camera.main.transform.position + (newPos-Camera.main.transform.position)*0.25f;
                    transform.position = newPos;
                    transform.rotation = Quaternion.LookRotation(-GetReflectDir(transform.forward), up);
                    transform.localScale = transform.localScale * 0.25f;
                    fontColor.gameObject.layer = LayerMask.NameToLayer("NoReflex");

                    transform.DOKill();
                    transform.DOScale(0.27f*0.25f, 0.5f)
                    .SetEase(Ease.OutBack, 2.5f).OnComplete(()=>{
                        EventHandler.Call_OnMirrorText(this);
                        DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, Color.white, 2f);
                    });
                });
            }
        }
        else
        {
            if(isFocus && !isRevealed)
            {
                OnMirrorTextHide();
            }
        }
        
        return isFocus;
    }
    //InDirection pointing to mirror, output direction point outside of mirror
    Vector3 GetReflectDir(in Vector3 inDir)
    {
        Vector3 n = Vector3.Dot(mirrorCenter.forward, inDir) * mirrorCenter.forward;
        return inDir - n*2;
    }
    public void OnMirrorTextHide()
    {
        isFocus = false;

        if(!isRevealed)
        {
            DOTween.Kill(this);
            DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, originColor, 0.5f).SetId(this);
            transform.DOKill();
            transform.DOScale(0.15f, 0.5f)
            .SetEase(Ease.OutQuad);
        }
    }
}
