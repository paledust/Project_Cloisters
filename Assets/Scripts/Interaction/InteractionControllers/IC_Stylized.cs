using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Stylized : IC_Basic
{
    [SerializeField] private Clickable_Planet clickablePlanet;
    [SerializeField] private CircleExpandingController circleExpandingController;
    protected override void LoadAssets()
    {
        base.LoadAssets();
        circleExpandingController.enabled = true;
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        circleExpandingController.enabled = false;
    }
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
