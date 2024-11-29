using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererSky : PerRendererBehavior
{
    public float UVFade;
    public float UVFadeSmooth;
    public float DetailStrength;
    public float DetailFade;
    public float DetailFadeSmooth;

    private readonly int UVFadeID = Shader.PropertyToID("_UVFade");
    private readonly int UVFadeSmoothID = Shader.PropertyToID("_UVFadeSmooth");
    private readonly int DetailStrengthID = Shader.PropertyToID("_DetailStrength");
    private readonly int DetailFadeID = Shader.PropertyToID("_DetailFade");
    private readonly int DetailFadeSmoothID = Shader.PropertyToID("_DetailFadeSmooth");

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(UVFadeID, UVFade);
        mpb.SetFloat(UVFadeSmoothID, UVFadeSmooth);
        mpb.SetFloat(DetailStrengthID, DetailStrength);
        mpb.SetFloat(DetailFadeID, DetailFade);
        mpb.SetFloat(DetailFadeSmoothID, DetailFadeSmooth);
    }
}
