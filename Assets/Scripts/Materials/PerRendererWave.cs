using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererWave : PerRendererBehavior
{
    public float wavePhase = 0;
    private readonly int WavePhaseID = Shader.PropertyToID("_WavePhase");
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(WavePhaseID, wavePhase);
    }
}
