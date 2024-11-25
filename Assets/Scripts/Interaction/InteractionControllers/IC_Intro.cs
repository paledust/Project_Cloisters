using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

[AddComponentMenu("Interaction Controller/IC_Intro")]
public class IC_Intro : IC_Basic
{
[Header("Interaction")]
    [SerializeField] private Clickable_Planet clickable_redPlanet;
    [SerializeField] private Transform centerPos;
    [SerializeField] private float angleTolrence = 10;
[Header("Planet")]
    [SerializeField] private Transform surroundPlanet;
    [SerializeField] private Transform centerPlanet;
[Header("End")]
    [SerializeField] private PlayableDirector endTimeline;

    protected override void OnInteractionStart()
    {
        this.enabled = true;
        clickable_redPlanet.EnableHitbox();
    }
    protected override void OnInteractionEnd()
    {
        this.enabled = false;
        clickable_redPlanet.DisableHitbox();
    }
    void Update(){
        Vector3 diff = surroundPlanet.position - centerPlanet.position;
        diff.y = 0;
        float angle = Vector3.SignedAngle(diff, Vector3.back, Vector3.up);
        angle = Mathf.Abs(angle);

        if(angle < angleTolrence && !m_isDone){
            EventHandler.Call_OnEndInteraction(this);
            StartCoroutine(coroutinePutPlanetToCenter());
        }
    }
    IEnumerator coroutinePutPlanetToCenter(){
        surroundPlanet.GetComponent<RotateAround>().enabled = false;

        Vector3 startPos = surroundPlanet.position;
        Vector3 finalPos = centerPos.position;
        yield return new WaitForLoop(0.5f, (t)=>{
            surroundPlanet.position = Vector3.Lerp(startPos, finalPos, EasingFunc.Easing.QuadEaseOut(t));
        });

        endTimeline.Play();
        yield return new WaitForSeconds((float)endTimeline.duration);
        yield return null;
        EventHandler.Call_OnInteractionUnreachable(this);
    }
}