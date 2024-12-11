using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleExpandingController : MonoBehaviour
{
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private float controlAgility = 5;
    [SerializeField] private float controlFactor = 0.1f;
    [SerializeField] private float radiusFlickFreq = 15;
    [SerializeField] private float externalNoiseMoveFactor = 1;
    [SerializeField] private Vector2 yawRange;
    [SerializeField] private AnimationCurve shrinkCurve;
    [SerializeField] private AnimationCurve noiseCurve;
    [SerializeField] private AnimationCurve noiseMinCurve;
    [SerializeField] private AnimationCurve radiusCurve;
    [SerializeField] private AnimationCurve radiusFlickAmpCurve;
    // [SerializeField, Range(0, 1)] 
    private float targetValue;
    private float controlValue;
    private float offsetAngle;
    private Vector3 initScale;
    public void ResetController(){
        offsetAngle = clickable_Planet.m_accumulateYaw;
        targetValue = controlValue = 0;

        expandCircle.noiseMin = noiseMinCurve.Evaluate(0);
        expandCircle.circleRadius = radiusCurve.Evaluate(0);
        expandCircle.noiseStrength = noiseCurve.Evaluate(0);
        expandCircle.externalNoiseMovement = 0;

        Vector3 scale = initScale;
        scale.y *= shrinkCurve.Evaluate(0);
        expandCircle.transform.localScale = scale;
    }
    void Start(){
        offsetAngle = 0;
        initScale = expandCircle.transform.localScale;
    }
    void Update()
    {
        targetValue = (-clickable_Planet.m_accumulateYaw+offsetAngle-yawRange.x)/(yawRange.y-yawRange.x);
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
    }
}
