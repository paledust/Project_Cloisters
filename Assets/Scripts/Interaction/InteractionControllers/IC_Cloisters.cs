using UnityEngine;

public class IC_Cloisters : IC_Basic
{
    [SerializeField] private Clickable_ObjectRotator rotator;
    [SerializeField] private float threasholdRotatorSpeed;
    [SerializeField] private float grawingSpeed = 1f;
    [SerializeField] private float switchAmount = 0.5f;
    private bool isDay = true;
    private float progress;

    protected void Update()
    {
        if(rotator.m_angularSpeed > threasholdRotatorSpeed)
        {
            progress += Time.deltaTime * grawingSpeed;
        }
        if(isDay && progress >= switchAmount)
        {
            isDay = false;
            //@to do: trigger day and night switch
        }
    }
}
