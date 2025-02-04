using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererCrystalParticle : PerRendererBehavior
{
    public float phaseOffset = 0;
    [SerializeField] private float phaseMultiplier = 100;
    [SerializeField] private float mix = 0.27f;
    [SerializeField] private float mixSmooth = 0.5f;
    private string phaseName = "_Phase";
    private string mixName = "_Mix";
    private string mixSmoothName = "_MixSmooth";

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(phaseName, phaseOffset*phaseMultiplier);
        mpb.SetFloat(mixName, mix);
        mpb.SetFloat(mixSmoothName, mixSmooth);
    }
}