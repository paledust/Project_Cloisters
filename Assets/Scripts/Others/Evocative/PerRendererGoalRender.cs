using DG.Tweening;
using DG.Tweening.Core;
using UnityEngine;

public class PerRendererGoalRender : PerRendererBehavior
{
    [SerializeField] private float rapidNoiseStrength;
    [SerializeField] private float stackNoiseStrength;

    private TweenerCore<float, float, DG.Tweening.Plugins.Options.FloatOptions> rapidNoiseTween;
    private const string rapidNoiseStrengthProperty = "_RapidNoiseStrength";
    private const string stackNoiseStrengthProperty = "_StackNoiseStrength";

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(rapidNoiseStrengthProperty, rapidNoiseStrength);
        mpb.SetFloat(stackNoiseStrengthProperty, stackNoiseStrength);
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
    public void StackUpNoise(int noiseLevel)
    {
        stackNoiseStrength = noiseLevel*1.5f;
    }
}