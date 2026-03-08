using System.Collections;
using DG.Tweening;
using EasingFunc;
using TMPro;
using UnityEngine;

public class ChargeText : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    [SerializeField, Range(0, 1)] private float chargeValue = 0;
    [SerializeField] private float chargeDim = 0.5f;
    [SerializeField] private float blinkFreq;
    [SerializeField] private ParticleSystem p_fireburst;

[Header("Fully Charge")]
    [SerializeField] private PerRendererColor perRendererColor;
    [SerializeField, ColorUsage(true, true)] private Color blinkColor;
    [SerializeField, ColorUsage(true, true)] private Color birghtColor;

    private float seed;

    void Awake()
    {
        tmp.alpha = 0;
        chargeValue = 0;
        seed = Random.value;
    }
    void Update()
    {
    //Blink Text
        if(chargeValue > 0)
        {
            float blink = Mathf.PerlinNoise(seed, Time.time * blinkFreq);
            blink = Mathf.Lerp(chargeValue*chargeValue*chargeDim, 1, blink);
            Color tempColor = blinkColor * blink;
            tempColor.a = 1;
            perRendererColor.hdrTint = tempColor;
        }
        else
        {
            perRendererColor.hdrTint = Color.black;
        }
    }
    public void BlinkText(float duration, AnimationCurve brightCurve)
    {
        tmp.DOFade(1, duration);
        StartCoroutine(coroutineFadeText(duration, brightCurve));
    }
    public void GetCharge(in float newCharge)
    {
        chargeValue = newCharge;
        chargeValue = Mathf.Clamp01(chargeValue);
        tmp.alpha = Mathf.Lerp(0.5f, 0.7f, chargeValue);
        transform.localScale = Vector3.one * Mathf.Lerp(0.9f, 0.95f, Easing.QuadEaseOut(chargeValue));

        if(chargeValue >= 1)
        {
            FullyCharged();
        }
    }
    void FullyCharged()
    {
        this.enabled = false;
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack, 4);
        tmp.DOFade(1, 0.2f);

        p_fireburst.Play();
        tmp.fontMaterial.SetFloat("_StencilComp", 8);
        perRendererColor.hdrTint = blinkColor;

        EventHandler.Call_OnChargeText();
    }

    public void PopoutText(float delay)
    {
        tmp.DOFade(0, 0.15f).SetDelay(delay).OnStart(() =>
        {
            p_fireburst.Play();
        });
    }
    IEnumerator coroutineFadeText(float duration, AnimationCurve curve)
    {
        yield return new WaitForLoop(duration, (t)=>{
            perRendererColor.hdrTint = Color.LerpUnclamped(Color.white, birghtColor, curve.Evaluate(t));
        });
        perRendererColor.hdrTint = Color.white;
        p_fireburst.Play();
    }
}