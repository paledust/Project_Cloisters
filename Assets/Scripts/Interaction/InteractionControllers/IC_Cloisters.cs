using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Cloisters : IC_Basic
{
    [SerializeField] private Clickable_CloisterSphere heroSphere;
    
    [Header("Main Feedback")]
    [SerializeField] private PerRendererCloistersDissolve shineDissolve;
    [SerializeField] private float threasholdRotatorSpeed;
    [SerializeField] private float grawingSpeed = 1f;
    
    [Header("Totem")]
    [SerializeField] private PlayableDirector cloistersTimeline;
    [SerializeField, ShowOnly] private float progress;

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
            progress += Time.deltaTime * grawingSpeed;
            shineDissolve.dissolveRadius = Mathf.Lerp(shineDissolve.dissolveRadius, 1, Time.deltaTime);
        }
        else
        {
            shineDissolve.dissolveRadius = Mathf.Lerp(shineDissolve.dissolveRadius, 0, Time.deltaTime);
        }
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