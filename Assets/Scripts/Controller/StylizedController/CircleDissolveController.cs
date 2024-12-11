using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDissolveController : MonoBehaviour
{
    [SerializeField] private IC_Stylized iC_Stylized;
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private float dissolveMaxAngle;
    [SerializeField] private float dissolveRadius;

    private float initAngle;
    private float initRadius;

    public void ResetController(){
        initAngle = clickable_Planet.m_accumulateYaw-clickable_Planet.m_angularSpeed*1;
        initRadius = expandCircle.circleRadius;
    }
    void Update()
    {
        float offset = (initAngle - clickable_Planet.m_accumulateYaw)/dissolveMaxAngle;
        offset = Mathf.Clamp01(offset);
        expandCircle.circleRadius = Mathf.Lerp(initRadius, dissolveRadius, offset);
        if(offset>=1){
            iC_Stylized.StartExpand();
        }
    }
}
