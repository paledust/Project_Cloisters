using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDissolveController : MonoBehaviour
{
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private float dissolveMaxAngle;
    [SerializeField] private float dissolveRadius;
    private float initAngle;
    private float initRadius;
    void OnEnable(){
        initAngle = clickable_Planet.m_accumulateYaw;
        initRadius = expandCircle.circleRadius;
    }
    void Update()
    {
        float offset = (initAngle - clickable_Planet.m_accumulateYaw)/dissolveMaxAngle;
        offset = Mathf.Clamp01(offset);
        expandCircle.circleRadius = Mathf.Lerp(initRadius, dissolveRadius, offset);
    }
}
