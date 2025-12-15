using UnityEngine;

public class IC_Cloisters : IC_Basic
{
    [SerializeField] private Clickable_CloisterSphere heroSphere;
    
    [Header("Main Feedback")]
    [SerializeField] private Animator shineDissolve;
    [SerializeField] private float threasholdRotatorSpeed;
    [SerializeField] private float switchAmount = 0.5f;
    [SerializeField] private float grawingSpeed = 1f;
    
    [Header("Totem")]
    [SerializeField] private PerRendererCloistersDissolve constructDissolve;
    [SerializeField] private PerRendererCloistersDissolve plantDissolve;
    [SerializeField] private PerRendererCloistersDissolve textDissolve;
    [SerializeField] private PerRendererCloistersDissolve sunDissolve;
    [SerializeField, ShowOnly] private float progress;

    private bool isDay = true;
    private bool isDissolveIn = false;
    private const string DISSOLVE_IN_BOOLEAN = "DissolveIn";

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
        if(isDay && progress >= switchAmount)
        {
            isDay = false;
            //@to do: trigger day and night switch
        }
    }
}