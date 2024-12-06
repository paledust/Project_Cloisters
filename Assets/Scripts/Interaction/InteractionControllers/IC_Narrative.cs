using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Narrative : IC_Basic
{
    [System.Serializable]
    public struct HeroCircleTransistor{
        public SpriteRenderer heroCircleSprite;
        public Color transitionColor;
    }
    [SerializeField] private RippleParticleController rippleParticleController;
    [SerializeField] private SmallCircleSpawner circleSpawner;
    [SerializeField] private NarrativeSpawner narrativeSpawner;
    [SerializeField] private ParticleSystem p_collideBurst;
    [SerializeField] private float effectiveCollisionStep = 3;
[Header("End")]
    [SerializeField] private float transition = 10;
    [SerializeField] private PlayableDirector TL_End;
[Header("Hero Circle Control")]
    [SerializeField] private HeroCircleTransistor[] heroCircleSprites;

    private bool isEnding = false;
    private float lastCollisionTime;
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
        lastCollisionTime = Time.time - effectiveCollisionStep;
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
        //Play Collision Particle
            p_collideBurst.transform.position = contact;
            p_collideBurst.transform.rotation = Quaternion.Euler(0,0,Vector3.SignedAngle(Vector3.right, diff, Vector3.forward));
            p_collideBurst.Play(true);
        //Check if collision too frequent
            if(Time.time - lastCollisionTime<=effectiveCollisionStep) return;

            lastCollisionTime = Time.time;

            collidedCircle.TriggerCollideRipple();
            StartCoroutine(CommonCoroutine.delayAction(()=>{
                narrativeSpawner.PlaceText();
                if(!isEnding){
                    isEnding = true;
                    for(int i=0; i<heroCircleSprites.Length; i++){
                        StartCoroutine(coroutineTransitionCircle(heroCircleSprites[i], transition));
                    }
                    StartCoroutine(coroutineEndInteraction());   
                }
            }, 0.5f));
        }
    }
    IEnumerator coroutineEndInteraction(){
        yield return new WaitForSeconds(transition);
        EventHandler.Call_OnEndInteraction(this);
        yield return new WaitForSeconds(2f);
        TL_End.Play();
        yield return new WaitForSeconds(1f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
    IEnumerator coroutineTransitionCircle(HeroCircleTransistor circleTransistor, float duration){
        Color initColor = circleTransistor.heroCircleSprite.color;
        yield return new WaitForLoop(duration, (t)=>{
            circleTransistor.heroCircleSprite.color = Color.Lerp(initColor, circleTransistor.transitionColor, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
}