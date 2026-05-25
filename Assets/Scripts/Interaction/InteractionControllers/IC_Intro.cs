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
    [SerializeField] private Clickable_ObjectRotator clickable_redPlanet;
    [SerializeField] private float alignTolrence = 10;
[Header("Planet")]
    [SerializeField] private Transform surroundPlanet;
    [SerializeField] private Transform centerPlanet;
[Header("End")]
    [SerializeField] private PlayableDirector endTimeline;

    private Vector3 lastSurroundPlanetPos;

    protected override void OnInteractionEnter()
    {
        this.enabled = true;
        motionController.enabled = true;
        camController.enabled = true;
        rotateController.enabled = true;
        clickable_redPlanet.EnableHitbox();
        UI_Manager.Instance.ChangeCursorColor(true);
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

        if(angle < alignTolrence && !m_isDone){
            EventHandler.Call_OnEndInteraction(this);
            StartCoroutine(coroutinePutPlanetToCenter());
        }
        lastSurroundPlanetPos = surroundPlanet.position;
    }
    IEnumerator coroutinePutPlanetToCenter(){
        var rotatAround = surroundPlanet.GetComponent<RotateAround>();
        rotatAround.enabled = false;

        float targetAngle = rotatAround.m_rotateAngle>0?180:-180;

        float currentAngularSpeed = rotatAround.angularSpeed;
        float currentAngle = rotatAround.m_rotateAngle;

        float leftAngle = targetAngle - currentAngle;
        float duration = leftAngle * 2 / currentAngularSpeed;

        yield return new WaitForLoop(duration, (t) =>
        {
            float speed = Mathf.Lerp(currentAngularSpeed, 0, t);
            rotatAround.StepSim(speed * Time.deltaTime);
        });

        endTimeline.Play();
        yield return new WaitForSeconds(1f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
}