using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Narrative : IC_Basic
{
    [SerializeField] private RippleParticleController rippleParticleController;
    protected override void LoadAssets()
    {
        base.LoadAssets();
        rippleParticleController.enabled = true;
    }
    protected override void UnloadAssets()
    {
        rippleParticleController.enabled = false;
    }
}
