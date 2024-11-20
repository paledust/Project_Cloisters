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
            GoToNextPlanet();
        }
    }
    void OnDisable(){
        EventHandler.E_OnPlanetReachPos -= GoToNextPlanet;
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
