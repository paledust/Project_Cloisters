using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Stylized : IC_Basic
{
    [SerializeField] private Clickable_Planet clickablePlanet;
    [SerializeField] private CircleExpandingController circleExpandingController;
    [SerializeField] private CircleExplodeController circleExplodeController;
    [SerializeField] private CircleDissolveController circleDissolveController;
    [SerializeField] private GeoFragmentController geoFragmentController;
    [SerializeField] private GeoTextController geoTextController;

    protected override void LoadAssets()
    {
        base.LoadAssets();
        StartExpand();
        geoFragmentController.enabled = true;
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        circleExpandingController.enabled = false;
        circleExplodeController.enabled = false;
        circleDissolveController.enabled = false;
        geoFragmentController.enabled = false;
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
        geoFragmentController.StartExpand();
        
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
    public void ExplodeToDissolveTransition(){
        int count = Random.Range(2, 4);
        for(int i=0; i<count; i++){
            geoTextController.ShowText();
        }
        geoFragmentController.StartDissolve();
    }
    public void OnAllTextOut(){
        EventHandler.Call_OnFlashInput();
        EventHandler.Call_OnEndInteraction(this);
        StartCoroutine(CommonCoroutine.delayAction(()=>{
            geoTextController.PutTextTogether();
        }, 1.2f));
    }
}
