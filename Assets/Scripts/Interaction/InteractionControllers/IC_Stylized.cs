using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Stylized : IC_Basic
{
    [SerializeField] private Clickable_Planet clickablePlanet;
    [SerializeField] private CircleExpandingController circleExpandingController;
    [SerializeField] private CircleExplodeController circleExplodeController;
    [SerializeField] private CircleDissolveController circleDissolveController;
    protected override void LoadAssets()
    {
        base.LoadAssets();
        circleExpandingController.enabled = true;
        circleDissolveController.enabled = false;
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        circleExpandingController.enabled = false;
        circleDissolveController.enabled = false;
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
    public void StartExpand(){
        circleExplodeController.ResetController();
        circleExpandingController.ResetController();
        clickablePlanet.FormSpring();

        circleDissolveController.enabled = false;
        circleExpandingController.enabled = true;
        circleExplodeController.enabled = true;
    }
    public void StartDissovle(){
        circleDissolveController.ResetController();

        circleDissolveController.enabled = true;
        circleExpandingController.enabled = false;
        circleExplodeController.enabled = false;
    }
}
