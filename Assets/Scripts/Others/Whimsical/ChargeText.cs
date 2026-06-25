using System.Collections;
using DG.Tweening;
using EasingFunc;
using SimpleAudioSystem;
using TMPro;
using UnityEngine;

public class ChargeText : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    [SerializeField, Range(0, 1)] private float chargeValue = 0;
    [SerializeField] private float chargeDim = 0.5f;
    [SerializeField] private float blinkFreq;
    [SerializeField] private ParticleSystem p_fireburst;
    [SerializeField] private ParticleSystem p_charge;

[Header("Fully Charge")]
    [SerializeField] private PerRendererColor perRendererColor;
    [SerializeField, ColorUsage(true, true)] private Color blinkColor;
    [SerializeField, ColorUsage(true, true)] private Color birghtColor;

[Header("Audio")]
    [SerializeField] private string sfxHide;
    [SerializeField] private string sfxReveal;
    [SerializeField] private string sfxCharge;
    [SerializeField] private float chargeVolume = 0.2f;
    [SerializeField] private float chargeLerpSpeed = 4f;
    [SerializeField] private float chargeFadeSpeed = 1f;
    [SerializeField] private AudioSource sfxChargeLoopSource;
    private bool isCharging;
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

        if(isCharging)
        {
            if(!sfxChargeLoopSource.isPlaying)
                AudioManager.Instance.PlaySFXLoop(sfxChargeLoopSource, sfxCharge, 0, 0);
            sfxChargeLoopSource.volume = Mathf.Lerp(sfxChargeLoopSource.volume, chargeVolume, Time.deltaTime*chargeLerpSpeed);
        }
        else
        {
            if(sfxChargeLoopSource.isPlaying)
            {
                sfxChargeLoopSource.volume = Mathf.Lerp(sfxChargeLoopSource.volume, 0, Time.deltaTime*chargeFadeSpeed);
                if(sfxChargeLoopSource.volume<0.01f)
                    sfxChargeLoopSource.Stop();
            }
        }
    }
    public void OnCharged()
    {
        if(!isCharging)
        {
            isCharging = true;
            p_charge.Play(true);
        }
    }
    public void OnNotCharged()
    {
        if(isCharging)
        {
            isCharging = false;
            p_charge.Stop(true);
        }
    }
    void FullyCharged()
    {
        this.enabled = false;
        OnNotCharged();
        sfxChargeLoopSource.DOFade(0, 1f);
        transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.OutBack, 4);
        tmp.DOFade(1, 0.2f);

        p_fireburst.Play();
        tmp.fontMaterial.SetFloat("_StencilComp", 8);
        perRendererColor.hdrTint = blinkColor;

        AudioManager.Instance.PlaySFX(sfxReveal, 1);

        EventHandler.Call_OnChargeText();
    }

    public void PopoutText(float delay)
    {
        tmp.DOFade(0, 0.15f).SetDelay(delay).OnStart(() =>
        {
            p_fireburst.Play();
            AudioManager.Instance.PlaySFX(sfxHide, 0.25f);
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