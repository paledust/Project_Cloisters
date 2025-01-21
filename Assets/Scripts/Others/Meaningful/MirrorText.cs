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
    private Color originColor;

    public char TextChar => textChar;

    void Awake()
    {
        originColor = fontColor.Tint;
    }
    public void OnMirrorTextFound()
    {
        DOTween.Kill(this);
        DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, foundColor, 0.5f).SetId(this);
    }
    public void OnMirrorTextHide()
    {
        DOTween.Kill(this);
        DOTween.To(()=>fontColor.Tint, x=>fontColor.Tint = x, originColor, 0.5f).SetId(this);
    }
}
