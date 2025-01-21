using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class MirrorText : MonoBehaviour
{
    [SerializeField, ColorUsage(true, true)] public Color foundColor;
    [SerializeField] private TextMeshPro tmp;
    [SerializeField] private char textChar;

    private PerRendererColor perRendererColor;
    private Color originColor;

    public char TextChar => textChar;

    void Awake()
    {
        perRendererColor = GetComponent<PerRendererColor>();
        originColor = perRendererColor.hdrTint;
    }
    public void OnMirrorTextFound()
    {
        DOTween.Kill(this);
        DOTween.To(()=>perRendererColor.hdrTint, x=>perRendererColor.hdrTint = x, foundColor, 0.5f).SetId(this);
    }
    public void OnMirrorTextHide()
    {
        DOTween.Kill(this);
        DOTween.To(()=>perRendererColor.hdrTint, x=>perRendererColor.hdrTint = x, originColor, 0.5f).SetId(this);
    }
}
