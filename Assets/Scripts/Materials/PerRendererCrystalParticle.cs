using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererCrystalParticle : PerRendererBehavior
{
    public float phaseOffset = 0;
    [SerializeField] private float phaseMultiplier = 100;
    private string phaseName = "_Phase";

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(phaseName, phaseOffset*phaseMultiplier);
    }
}