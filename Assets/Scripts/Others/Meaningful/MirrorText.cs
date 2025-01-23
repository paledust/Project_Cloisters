using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MirrorText : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] public Color foundColor;
    [SerializeField] private char textChar;
    [SerializeField] private PerRendererStylizedShade fontColor;
    [SerializeField] private float focusRadius = 0.5f;
    [SerializeField] private float searchRadius = 2f;
    private Color originColor;
    private bool isFocus = false;
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
        Vector3 t = diff - transform.forward * Vector3.Dot(transform.forward, diff);

        Debug.Log(focusFactor);
        focusFactor = Mathf.Clamp01((t.magnitude-focusRadius)/(searchRadius-focusRadius));

        if(t.sqrMagnitude<focusRadius*focusRadius)
        {
            if(!isFocus)
            {
                isFocus = true;
                DOTween.Kill(this);
                DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, foundColor, 0.5f).SetId(this);
            }
        }
        else
        {
            if(isFocus)
            {
                OnMirrorTextHide();
            }
        }
        
        return isFocus;
    }
    public void OnMirrorTextHide()
    {
        isFocus = false;
        DOTween.Kill(this);
        DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, originColor, 0.5f).SetId(this);
    }
}
