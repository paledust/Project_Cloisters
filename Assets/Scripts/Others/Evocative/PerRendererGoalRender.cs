using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class PerRendererGoalRender : PerRendererBehavior
{
    [SerializeField] private float rapidNoiseStrength;
    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> rapidNoiseTween;
    private const string rapidNoiseStrengthProperty = "_RapidNoiseStrength";
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(rapidNoiseStrengthProperty, rapidNoiseStrength);
    }
    public void ImpulseRapidNoise(float strength, float kickIn = 0.1f, float fadeOut = 0.5f)
    {
        DOTween.Kill(this);
        DOTween.To(() => rapidNoiseStrength, x => rapidNoiseStrength = x, strength, kickIn)
               .OnComplete(() =>
               {
                   DOTween.To(() => rapidNoiseStrength, x => rapidNoiseStrength = x, 0f, fadeOut).SetId(this);
               }).SetId(this);
    }
}