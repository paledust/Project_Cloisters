using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Stylized : IC_Basic
{
    [SerializeField] private Clickable_ObjectRotator clickablePlanet;
    [SerializeField] private CircleExpandingController circleExpandingController;
    [SerializeField] private CircleExplodeController circleExplodeController;
    [SerializeField] private CircleDissolveController circleDissolveController;
    [SerializeField] private GeoFragmentController geoFragmentController;
    [SerializeField] private GeoTextController geoTextController;

    private bool transitioning = false;

    protected override void LoadAssets()
    {
        base.LoadAssets();
        StartExpand();
        geoFragmentController.enabled = true;
        geoTextController.enabled = true;
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        circleExpandingController.enabled = false;
        circleExplodeController.enabled = false;
        circleDissolveController.enabled = false;
        geoFragmentController.enabled = false;
        geoTextController.enabled = false;
    }
    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        transitioning = false;
        clickablePlanet.EnableHitbox();
        clickablePlanet.onClick += CompleteExpandToDissolve;
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        clickablePlanet.DisableHitbox();
        clickablePlanet.onClick -= CompleteExpandToDissolve;
    }
    void CompleteExpandToDissolve(){
        if(transitioning){
            transitioning = false;
            geoFragmentController.StartDissolving();
            circleDissolveController.ResetController();
            circleDissolveController.enabled = true;
            circleExpandingController.enabled = false;
            circleExplodeController.enabled = false;
        }
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
    public void ExplodeToDissolveTransition(){
        int count = Random.Range(2, 4);
        for(int i=0; i<count; i++){
            geoTextController.ShowText();
        }
        geoFragmentController.StartTransition();
        transitioning = true;
    }
    public void OnAllTextOut(){
        EventHandler.Call_OnFlashInput();
        EventHandler.Call_OnEndInteraction(this);
        StartCoroutine(CommonCoroutine.delayAction(()=>{
            geoTextController.PutTextTogether();
        }, 1.2f));
    }
}
