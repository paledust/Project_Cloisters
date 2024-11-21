using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Plan_ZoomToBluePlanet : MonoBehaviour
{
    [SerializeField] private Clickable_Planet clickable_redPlanet;
    [SerializeField] private PlayableDirector endTimeline;
[Header("Interaction")]
    [SerializeField] private float angleTolrence = 10;
    [SerializeField] private Transform centerPos;
[Header("Planet")]
    [SerializeField] private Transform surroundPlanet;
    [SerializeField] private Transform centerPlanet;

    private bool isDone = false;

    void OnEnable(){
        EventHandler.E_OnPlanetReachPos += GoToNextPlanet;
    }
    void Update(){
        Vector3 diff = surroundPlanet.position - centerPlanet.position;
        diff.y = 0;
        float angle = Vector3.SignedAngle(diff, Vector3.back, Vector3.up);
        angle = Mathf.Abs(angle);

        if(angle < angleTolrence && !isDone){
            isDone = true;
            this.enabled = false;
            StartCoroutine(coroutineNextPlanet());
        }
    }
    void OnDisable(){
        EventHandler.E_OnPlanetReachPos -= GoToNextPlanet;
    }
    IEnumerator coroutineNextPlanet(){
        clickable_redPlanet.DisableHitbox();
        surroundPlanet.GetComponent<RotateAround>().enabled = false;

        Vector3 startPos = surroundPlanet.position;
        Vector3 finalPos = centerPos.position;
        yield return new WaitForLoop(0.5f, (t)=>{
            surroundPlanet.position = Vector3.Lerp(startPos, finalPos, EasingFunc.Easing.QuadEaseOut(t));
        });

        EventHandler.Call_OnTransitionBegin();
        endTimeline.Play();
        StartCoroutine(CommonCoroutine.delayAction(()=>{
            EventHandler.Call_OnTransitionEnd();
        },(float)endTimeline.duration));
    }
    
    void GoToNextPlanet(){
        EventHandler.Call_OnTransitionBegin();
        clickable_redPlanet.DisableHitbox();
        endTimeline.Play();
        StartCoroutine(CommonCoroutine.delayAction(()=>{
            EventHandler.Call_OnTransitionEnd();
        },(float)endTimeline.duration));
    }
}
