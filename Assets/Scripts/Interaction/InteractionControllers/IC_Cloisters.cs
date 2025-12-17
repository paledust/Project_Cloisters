using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Cloisters : IC_Basic
{
    [SerializeField] private Clickable_CloisterSphere heroSphere;
    
    [Header("Main Feedback")]
    [SerializeField] private PerRendererCloistersDissolve shineDissolve;
    [SerializeField] private float threasholdRotatorSpeed;
    [SerializeField] private float switchAmount = 0.5f;
    [SerializeField] private float grawingSpeed = 1f;
    
    [Header("Totem")]
    [SerializeField] private PlayableDirector cloistersTimeline;
    [SerializeField, ShowOnly] private float progress;

    private float duration;
    private bool isHovering;

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        cloistersTimeline.Play();
        duration = (float)cloistersTimeline.duration;

        isHovering = true;
    }
    protected void Update()
    {
        if(isHovering && !PlayerManager.Instance.m_isHovering)
        {
            isHovering = false;
            UI_Manager.Instance.ChangeCursorMono(false);
        }
        if(!isHovering && PlayerManager.Instance.m_isHovering)
        {
            isHovering = true;
            UI_Manager.Instance.ChangeCursorMono(true);
        }

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