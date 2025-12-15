using UnityEngine;
using UnityEngine.Playables;

public class IC_Cloisters : IC_Basic
{
    [SerializeField] private Clickable_CloisterSphere heroSphere;
    
    [Header("Main Feedback")]
    [SerializeField] private Animator shineDissolve;
    [SerializeField] private float threasholdRotatorSpeed;
    [SerializeField] private float switchAmount = 0.5f;
    [SerializeField] private float grawingSpeed = 1f;
    
    [Header("Totem")]
    [SerializeField] private PlayableDirector cloistersTimeline;
    [SerializeField, ShowOnly] private float progress;

    private float duration;
    private bool isDissolveIn = false;
    private const string DISSOLVE_IN_BOOLEAN = "DissolveIn";

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        cloistersTimeline.Play();
        duration = (float)cloistersTimeline.duration;
    }
    protected void Update()
    {
        if(heroSphere.m_angularSpeed > threasholdRotatorSpeed)
        {
            progress += Time.deltaTime * grawingSpeed;
            if(!isDissolveIn)
            {
                isDissolveIn = true;
                shineDissolve.SetBool(DISSOLVE_IN_BOOLEAN, isDissolveIn);
            }
        }
        else
        {
            if(isDissolveIn)
            {
                isDissolveIn = false;
                shineDissolve.SetBool(DISSOLVE_IN_BOOLEAN, isDissolveIn);
            }
        }
        cloistersTimeline.playableGraph.GetRootPlayable(0).SetTime(progress*duration);
    }
}