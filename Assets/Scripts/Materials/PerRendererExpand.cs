using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererExpand : PerRendererBehavior
{
    public float circleRadius = 0.2f;
    public float noiseStrength = 0.15f;
    public float noiseMin = 0;

    private static readonly int NoiseMinID = Shader.PropertyToID("_NoiseMin");
    private static readonly int CircleRadiusID = Shader.PropertyToID("_CircleRadius");
    private static readonly int NoiseStrengthID = Shader.PropertyToID("_NoiseStrength");

    protected override void UpdateProperties()
    {
        base.UpdateProperties();

        mpb.SetFloat(NoiseMinID, noiseMin);
        mpb.SetFloat(CircleRadiusID, circleRadius);
        mpb.SetFloat(NoiseStrengthID, noiseStrength);
    }
}
