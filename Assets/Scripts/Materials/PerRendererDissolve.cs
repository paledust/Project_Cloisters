using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererDissolve : PerRendererBehavior
{
    public float DissolveStart = 0;
    private readonly int DissolveStartID = Shader.PropertyToID("_DissolveStart");

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(DissolveStartID, DissolveStart);
    }

}
