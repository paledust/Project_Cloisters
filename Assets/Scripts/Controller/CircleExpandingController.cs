using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleExpandingController : MonoBehaviour
{
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private float controlAgility = 5;
    [SerializeField] private Vector2 controlYawRange;
    [SerializeField] private AnimationCurve noiseCurve;
    [SerializeField] private AnimationCurve radiusCurve;

    private float controlValue;

    void Update()
    {
        float targetValue = (Mathf.Abs(clickable_Planet.m_accumulateYaw)-controlYawRange.x)/(controlYawRange.y-controlYawRange.x);
        targetValue = Mathf.Clamp01(targetValue);
        controlValue = Mathf.Lerp(controlValue, targetValue, Time.deltaTime*controlAgility);

        expandCircle.circleRadius = radiusCurve.Evaluate(controlValue);
        expandCircle.noiseStrength = noiseCurve.Evaluate(controlValue);
    }
}
