using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererWavyColor : PerRendererBehavior
{
    [SerializeField] private Color darkColor = Color.white;
    [SerializeField] private Color brightColor = Color.white;
    private static readonly int DarkColorID = Shader.PropertyToID("_DarkColor");
    private static readonly int BrightColorID = Shader.PropertyToID("_BrightColor");

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetColor(DarkColorID, darkColor);
        mpb.SetColor(BrightColorID, brightColor);
    }
}
