using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[AddComponentMenu("Interaction Controller/IC_Intro")]
public class IC_Intro : IC_Basic
{
[Header("Controller")]
    [SerializeField] private MotionSetController motionController;
    [SerializeField] private PlanetCameraController camController;
    [SerializeField] private RotateAroundController rotateController;
[Header("Interaction")]
    [SerializeField] private Clickable_Planet clickable_redPlanet;
    [SerializeField] private Transform centerPos;
    [SerializeField] private float angleTolrence = 10;
[Header("Planet")]
    [SerializeField] private Transform surroundPlanet;
    [SerializeField] private Transform centerPlanet;
[Header("End")]
    [SerializeField] private PlayableDirector endTimeline;

    private Vector3 lastSurroundPlanetPos;

    protected override void OnInteractionStart()
    {
        this.enabled = true;
        motionController.enabled = true;
        camController.enabled = true;
        rotateController.enabled = true;
        clickable_redPlanet.EnableHitbox();
    }
    protected override void OnInteractionEnd()
    {
        this.enabled = false;
        clickable_redPlanet.DisableHitbox();
    }
    protected override void UnloadAssets()
    {
        base.UnloadAssets();
        motionController.enabled = false;
        camController.enabled = false;
        rotateController.enabled = false;
    }
    void Update(){
        Vector3 diff = surroundPlanet.position - centerPlanet.position;
        diff.y = 0;
        float angle = Vector3.SignedAngle(diff, Vector3.back, Vector3.up);
        angle = Mathf.Abs(angle);

        if(angle < angleTolrence && !m_isDone){
            EventHandler.Call_OnEndInteraction(this);
            StartCoroutine(coroutinePutPlanetToCenter((surroundPlanet.position-lastSurroundPlanetPos)/Time.deltaTime));
        }
        lastSurroundPlanetPos = surroundPlanet.position;
    }
    IEnumerator coroutinePutPlanetToCenter(Vector3 startVelocity){
        surroundPlanet.GetComponent<RotateAround>().enabled = false;

        Vector3 startPos = surroundPlanet.position;
        Vector3 finalPos = centerPos.position;
        Vector3 startVel = startVelocity;
        Vector3 endVel   = (finalPos-startPos)*0.01f;
        Vector3 dynamicPos = startPos;
        yield return new WaitForLoop(0.5f, (t)=>{
            dynamicPos = dynamicPos + Vector3.Slerp(startVel, endVel, t)*Time.deltaTime;
            Vector3 staticPos  = Vector3.Lerp(startPos, finalPos, EasingFunc.Easing.SmoothInOut(t));
            surroundPlanet.position = Vector3.Lerp(dynamicPos, staticPos, EasingFunc.Easing.SmoothInOut(t));
        });

        endTimeline.Play();
        yield return new WaitForSeconds(1f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
}