using System;
using System.Collections;
using UnityEngine;

public class CircleExpandingController : MonoBehaviour
{
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private float controlAgility = 5;
    [SerializeField] private float controlFactor = 0.1f;
    [SerializeField] private float radiusFlickFreq = 15;
    [SerializeField] private float externalNoiseMoveFactor = 1;
    [SerializeField] private float expandCircleRadius = 1.2f;

    [Header("Animation Curves")]
    [SerializeField] private AnimationCurve shrinkCurve;
    [SerializeField] private AnimationCurve noiseCurve;
    [SerializeField] private AnimationCurve noiseMinCurve;
    [SerializeField] private AnimationCurve radiusCurve;
    [SerializeField] private AnimationCurve radiusFlickAmpCurve;

    [Header("Sphere Scale")]
    [SerializeField] private Transform sphereTrans;
    [SerializeField] private float startSphereScale = 0.25f;
    [SerializeField] private float finalScale = 0.3f;

    private float controlValue;
    private Vector3 initScale;

    void Awake()
    {
        initScale = expandCircle.transform.localScale;
    }
    public void ResetController()
    {
        controlValue = 0;
        expandCircle.noiseMin = noiseMinCurve.Evaluate(0);
        expandCircle.circleRadius = radiusCurve.Evaluate(0);
        expandCircle.noiseStrength = noiseCurve.Evaluate(0);
        expandCircle.externalNoiseMovement = 0;

        Vector3 scale = initScale;
        scale.y *= shrinkCurve.Evaluate(0);
        expandCircle.transform.localScale = scale;
    }
    public void UpdateExpand(float targetValue)
    {
        targetValue = Mathf.Clamp01(targetValue);
        controlValue = Mathf.Lerp(controlValue, targetValue, Time.deltaTime*controlAgility);

        float radiusFlickAmp = radiusFlickAmpCurve.Evaluate(controlValue);
        float radiusFlick = radiusFlickAmp*Mathf.PerlinNoise(radiusFlickFreq*Time.time, 0.38719123f);
        expandCircle.noiseMin = noiseMinCurve.Evaluate(controlValue);
        expandCircle.circleRadius = radiusCurve.Evaluate(controlValue) + radiusFlick;
        expandCircle.noiseStrength = noiseCurve.Evaluate(controlValue);
        expandCircle.externalNoiseMovement += controlFactor * externalNoiseMoveFactor;
        Vector3 scale = initScale;
        scale.y *= shrinkCurve.Evaluate(controlValue);
        expandCircle.transform.localScale = scale;

        sphereTrans.localScale = Vector3.one * Mathf.Lerp(startSphereScale, finalScale, targetValue);
    }
    public void ExpandCircleOut(Action OnExpandComplete)
    {
        StartCoroutine(coroutineExpand(OnExpandComplete));
    }
    IEnumerator coroutineExpand(Action onComplete)
    {
        float startRadius = expandCircle.circleRadius;
        yield return new WaitForLoop(2f, (t) =>
        {
            float _t = EasingFunc.Easing.CircEaseOut(t);
            expandCircle.circleRadius = Mathf.Lerp(startRadius, expandCircleRadius, _t);
        });
        expandCircle.circleRadius = expandCircleRadius;
        onComplete?.Invoke();
        yield return null;
    }
}
