using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Stylized : IC_Basic
{
    [SerializeField] private Clickable_Planet clickablePlanet;
    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        clickablePlanet.EnableHitbox();
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        clickablePlanet.DisableHitbox();
    }
}
