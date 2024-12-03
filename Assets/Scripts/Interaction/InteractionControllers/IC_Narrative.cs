using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Narrative : IC_Basic
{
    [SerializeField] private RippleParticleController rippleParticleController;
    [SerializeField] private SmallCircleSpawner circleSpawner;
    [SerializeField] private ExplodeCircleSpawner explodeCircleSpawner;
[Header("Collision")]
    [SerializeField] private NarrativeText narrativeText;

    private Clickable_Circle lastCircle;

    protected override void LoadAssets()
    {
        base.LoadAssets();
        rippleParticleController.enabled = true;
        circleSpawner.enabled = true;
    }
    protected override void UnloadAssets()
    {
        rippleParticleController.enabled = false;
    }
    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        EventHandler.E_OnControlCircle += OnControlCircleHandler;
        EventHandler.E_OnClickableCircleCollide += OnCircleCollide;
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        circleSpawner.enabled = false;
        EventHandler.E_OnControlCircle -= OnControlCircleHandler;
        EventHandler.E_OnClickableCircleCollide -= OnCircleCollide;
    }
    void OnControlCircleHandler(Clickable_Circle circle)=>lastCircle = circle;
    void OnCircleCollide(Clickable_Circle collidedCircle){
        if(collidedCircle != lastCircle && collidedCircle.IsGrownCircle){
            // EventHandler.Call_OnEndInteraction(this);
            collidedCircle.TriggerCollideRipple();
            spawnCircle(Random.Range(2,5), collidedCircle.transform);
            StartCoroutine(CommonCoroutine.delayAction(()=>{
                narrativeText.gameObject.SetActive(true);
                narrativeText.FadeInText();
            }, 0.5f));
        }
    }
    void spawnCircle(int amount, Transform spawnTrans){
        // for(int i=0; i<amount; i++){
        //     var go = explodeCircleSpawner.SpawnCircle(spawnTrans);
        // }
    }
}