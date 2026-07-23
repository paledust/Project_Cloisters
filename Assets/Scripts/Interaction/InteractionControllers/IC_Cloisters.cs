using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Cloisters : IC_Basic
{
    [Header("Cloisters")]
    [SerializeField] private Clickable_CloisterSphere heroSphere;
    
    [Header("Main Feedback")]
    [SerializeField] private PerRendererCloistersDissolve shineDissolve;
    [SerializeField] private float threasholdRotatorSpeed;
    [SerializeField] private float maxProgressSpeed = 1f;
    [SerializeField] private float progressLerp = 10f;
    
    [Header("Totem")]
    [SerializeField] private PlayableDirector cloistersTimeline;

    private float progress;
    private float progressSpeed;
    private float duration;
    private Playable rootPlayable;

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        this.enabled = true;
        cloistersTimeline.Play();
        rootPlayable = cloistersTimeline.playableGraph.GetRootPlayable(0);
        rootPlayable.SetSpeed(0);
        duration = (float)cloistersTimeline.duration;
        UI_Manager.Instance.ChangeCursorColor(false);
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        this.enabled = false;
    }
    protected void Update()
    {
        bool speedGate = heroSphere.m_angularSpeed > threasholdRotatorSpeed;
        progressSpeed = Mathf.Lerp(progressSpeed, speedGate?maxProgressSpeed:0, Time.deltaTime * progressLerp);
        shineDissolve.dissolveRadius = Mathf.Lerp(shineDissolve.dissolveRadius, speedGate?1:0, Time.deltaTime);

        progress += Time.deltaTime * progressSpeed;
        rootPlayable.SetTime(progress);
    }
    public void TL_Signal_AutoPlay()
    {
        StartCoroutine(coroutineAutoPlayTimeline());  
    }
    IEnumerator coroutineAutoPlayTimeline()
    {
        this.enabled = false;
        EventHandler.Call_OnTransitionBegin();
        EventHandler.Call_OnEndInteraction(this);

        float startSpeed = progressSpeed;
        float speed = startSpeed;

        for(; progress<duration; progress += Time.deltaTime * speed)
        {
            if(speed < 1)
            {
                speed += Time.deltaTime*0.5f;
                speed = Mathf.Min(1, speed);
            }
            progress = Mathf.Min(duration, progress);
            rootPlayable.SetTime(progress);
            cloistersTimeline.Evaluate();
            yield return null;
        }
        progress = duration;
    }
}