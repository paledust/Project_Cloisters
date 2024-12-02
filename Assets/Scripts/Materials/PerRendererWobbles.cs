using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererWobbles : PerRendererBehavior
{
    public float WobbleStrength = 0;
    private readonly int WobbleStrengthID = Shader.PropertyToID("_WobbleStrength");
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(WobbleStrengthID, WobbleStrength);
    }
}
