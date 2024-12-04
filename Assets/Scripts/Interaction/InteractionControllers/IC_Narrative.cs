using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Narrative : IC_Basic
{
    [SerializeField] private RippleParticleController rippleParticleController;
    [SerializeField] private SmallCircleSpawner circleSpawner;
    [SerializeField] private NarrativeSpawner narrativeSpawner;
    [SerializeField] private ParticleSystem p_collideBurst;

    private Clickable_Circle lastCircle;

    protected override void LoadAssets()
    {
        base.LoadAssets();
        rippleParticleController.enabled = true;
        narrativeSpawner.enabled = true;
        circleSpawner.enabled = true;
    }
    protected override void UnloadAssets()
    {
        rippleParticleController.enabled = false;
        narrativeSpawner.enabled = false;
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
    void OnCircleCollide(Clickable_Circle collidedCircle, Vector3 contact, Vector3 diff){
        if(collidedCircle != lastCircle && collidedCircle.IsGrownCircle){
            collidedCircle.TriggerCollideRipple();
            p_collideBurst.transform.position = contact;
            p_collideBurst.transform.rotation = Quaternion.Euler(0,0,Vector3.SignedAngle(Vector3.right, diff, Vector3.forward));
            p_collideBurst.Play(true);
            StartCoroutine(CommonCoroutine.delayAction(()=>{
                narrativeSpawner.PlaceText();
            }, 0.5f));
        }
    }
}