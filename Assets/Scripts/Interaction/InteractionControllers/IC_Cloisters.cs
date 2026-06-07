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

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        this.enabled = true;
        cloistersTimeline.Play();
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
        if(heroSphere.m_angularSpeed > threasholdRotatorSpeed)
        {
            progressSpeed = Mathf.Lerp(progressSpeed, maxProgressSpeed, Time.deltaTime * progressLerp);
            shineDissolve.dissolveRadius = Mathf.Lerp(shineDissolve.dissolveRadius, 1, Time.deltaTime);
        }
        else
        {
            progressSpeed = Mathf.Lerp(progressSpeed, 0, Time.deltaTime * progressLerp);
            shineDissolve.dissolveRadius = Mathf.Lerp(shineDissolve.dissolveRadius, 0, Time.deltaTime);
        }

        progress += Time.deltaTime * progressSpeed;
        cloistersTimeline.playableGraph.GetRootPlayable(0).SetTime(progress);
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
        float currentProgress = progress;
        yield return new WaitForLoop(6f, t =>
        {
            progress += Time.deltaTime;
            progress = Mathf.Min(duration, progress);
        });
        progress = duration;
    }
}