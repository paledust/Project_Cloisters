using DG.Tweening;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PerRendererColor : PerRendererBehavior
{
    [ColorUsage(true, false)]
    public Color tint = Color.white;
    [ColorUsage(true, true)]
    public Color hdrTint = Color.white;
    [SerializeField] protected bool useHDR;
    [SerializeField] protected string ColorName = "_Color";
    protected override void UpdateProperties()
    {
        mpb.SetColor(ColorName, useHDR?hdrTint:tint);
    }
}
