using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChargeText : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    [SerializeField, Range(0, 1)] private float chargeValue = 0;
    [SerializeField] private float blinkFreq;
    public float phase;

    private float seed;
    private float maxBright;
    private bool fullyCharged = false;
    private bool isCharging = false;

    public float ChargeValue => chargeValue;

    void Awake()
    {
        tmp.alpha = 0;
        chargeValue = 0;
        seed = Random.value;
    }
    void Update()
    {
    //Charge Check
        if(chargeValue>=1)
        {
            if(!fullyCharged)
            {
                fullyCharged = true;
            }
        }
        else
        {
            if(fullyCharged)
            {
                fullyCharged = false;
            }
        }
    //Change Render
        if(chargeValue > 0)
        {
            float blink = Mathf.PerlinNoise(seed, Time.time * blinkFreq);
            blink = Mathf.Lerp(chargeValue*chargeValue*0.5f, maxBright, blink);
            tmp.alpha = blink;
        }
        else
        {
            tmp.alpha = 0;
        }
    }
    public void GetCharge(in float totalCharge)
    {
        chargeValue = totalCharge-phase;
        chargeValue = Mathf.Clamp01(chargeValue);
        if(chargeValue>0)
        {
            if(!isCharging)
            {
                isCharging = true;
                DOTween.Kill(this);
                DOTween.To(()=>maxBright, x=>maxBright = x, 1, 0.35f).SetId(this);
            }
        }
        else
        {
            if(isCharging)
            {
                isCharging = false;
                DOTween.Kill(this);
                DOTween.To(()=>maxBright, x=>maxBright = x, 0, 0.35f).SetId(this);
            }   
        }
    }
}
