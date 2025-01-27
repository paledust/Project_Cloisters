using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System.Runtime.CompilerServices;

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
    public void OnMirrorTextFound(){}
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
                DOTween.Kill(this);
                DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, foundColor, 1f)
                .SetEase(Ease.InQuad)
                .SetId(this)
                .OnComplete(()=>{
                    isFocus = false;
                    isRevealed = true;
                    hitbox.enabled = false;
                    p_spot.Play();
                    EventHandler.Call_OnMirrorText(textChar);
                    transform.DOKill();
                    transform.DOScale(0.27f, 0.5f)
                    .SetEase(Ease.OutBack, 2.5f);
                });
                transform.DOKill();
                transform.DOScale(0.18f, 1f)
                .SetEase(Ease.InQuad);
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
