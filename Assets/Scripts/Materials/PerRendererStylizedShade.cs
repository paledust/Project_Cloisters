using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererStylizedShade : PerRendererBehavior
{
    [ColorUsage(false)] public Color DarkColor = Color.black;
    [ColorUsage(false)] public Color BrightColor = Color.white;
    [ColorUsage(false, true)] public Color Tint = Color.white;
    public float ShadeMin = 0;
    public float ShadeSmooth = 1;

    private static readonly int DarkColorID = Shader.PropertyToID("_DarkColor");
    private static readonly int BrightColorID = Shader.PropertyToID("_BrightColor");
    private static readonly int ShadeMinID = Shader.PropertyToID("_ShadeMin");
    private static readonly int ShadeSmoothID = Shader.PropertyToID("_ShadeSmooth");

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetColor(DarkColorID, DarkColor);
        mpb.SetColor(BrightColorID, BrightColor);
        mpb.SetColor(COLOR_ID, Tint);
        mpb.SetFloat(ShadeMinID, ShadeMin);
        mpb.SetFloat(ShadeSmoothID, ShadeSmooth);
    }
}
