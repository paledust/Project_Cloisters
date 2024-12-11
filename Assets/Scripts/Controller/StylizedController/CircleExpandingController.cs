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
    [SerializeField] private AnimationCurve noiseCurve;
    [SerializeField] private AnimationCurve noiseMinCurve;
    [SerializeField] private AnimationCurve radiusCurve;
    [SerializeField] private AnimationCurve radiusFlickAmpCurve;
    // [SerializeField, Range(0, 1)] 
    private float targetValue;
    private float controlValue;

    void Update()
    {
        targetValue += controlFactor * clickable_Planet.m_angularSpeed * Time.deltaTime;
        targetValue = Mathf.Clamp01(targetValue);
        controlValue = Mathf.Lerp(controlValue, targetValue, Time.deltaTime*controlAgility);

        float radiusFlickAmp = radiusFlickAmpCurve.Evaluate(controlValue);
        float radiusFlick = radiusFlickAmp*Mathf.PerlinNoise(radiusFlickFreq*Time.time, 0.38719123f);
        expandCircle.noiseMin = noiseMinCurve.Evaluate(controlValue);
        expandCircle.circleRadius = radiusCurve.Evaluate(controlValue) + radiusFlick;
        expandCircle.noiseStrength = noiseCurve.Evaluate(controlValue);
    }
    IEnumerator coroutineExplodeExpand(float duration){
        yield return new WaitForLoop(duration, (t)=>{
            
        });
    }
}
