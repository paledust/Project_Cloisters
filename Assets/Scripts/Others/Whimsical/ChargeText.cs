using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class ChargeText : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    [SerializeField, Range(0, 1)] private float chargeValue = 0;
    [SerializeField] private float chargeDim = 0.5f;
    [SerializeField] private float blinkFreq;
    [SerializeField] private float fadeInTime = 0.35f;
    [SerializeField] private float fadeOutTime;
[Header("Fully Charge")]
    [SerializeField] private PerRendererColor perRendererColor;
    [SerializeField, ColorUsage(true, true)] private Color birghtColor;
    public float phase;

    private float seed;
    private float maxBright;
    private bool charged = false;
    private bool fullyCharged = false;
    private bool isCharging = false;

    void Awake()
    {
        tmp.alpha = 0;
        chargeValue = 0;
        seed = Random.value;
    }
    void Update()
    {
        if(fullyCharged)
        {
            this.enabled = false;
            return;
        }

        if(chargeValue>=1)
        {
            if(!charged)
            {
                charged = true;
                EventHandler.Call_OnChargeText(true);
            }
        }
        else
        {
            if(charged)
            {
                charged = false;
                EventHandler.Call_OnChargeText(false);
            }
        }
    //Change Render
        if(chargeValue > 0)
        {
            float blink = Mathf.PerlinNoise(seed, Time.time * blinkFreq);
            blink = Mathf.Lerp(chargeValue*chargeValue*chargeDim, maxBright, blink);
            tmp.alpha = blink;
        }
        else
        {
            tmp.alpha = 0;
        }
    }
    public void StayCharged(float duration, AnimationCurve brightCurve)
    {
        fullyCharged = true;
        DOTween.Kill(this);
        tmp.DOFade(1, duration);
        StartCoroutine(coroutineFadeText(duration, brightCurve));
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
                DOTween.To(()=>maxBright, x=>maxBright = x, 1, fadeInTime).SetId(this);
            }
        }
        else
        {
            if(isCharging)
            {
                isCharging = false;
                DOTween.Kill(this);
                DOTween.To(()=>maxBright, x=>maxBright = x, 0, fadeOutTime).SetId(this);
            }   
        }
    }
    IEnumerator coroutineFadeText(float duration, AnimationCurve curve)
    {
        yield return new WaitForLoop(duration, (t)=>{
            perRendererColor.hdrTint = Color.LerpUnclamped(Color.white, birghtColor, curve.Evaluate(t));
        });
        perRendererColor.hdrTint = Color.white;
    }
}
