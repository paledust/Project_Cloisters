using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleDissolveController : MonoBehaviour
{
    [SerializeField] private IC_Stylized iC_Stylized;
    [SerializeField] private Clickable_ObjectRotator clickable_Planet;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private float dissolveMaxAngle;
    [SerializeField] private float dissolveRadius;
    [SerializeField] private float shrinkRadius;
    
    private float initAngle;
    private float initRadius;
    private float dissolveMinAngle;

    public void ResetController(){
        dissolveMinAngle = clickable_Planet.m_angularSpeed*0f;
        initAngle = clickable_Planet.m_accumulateYaw;
        initRadius = expandCircle.circleRadius;
    }
    void Update()
    {
        float angleOffset = -(clickable_Planet.m_accumulateYaw-initAngle);
        float offset = (Mathf.Abs(angleOffset)-dissolveMinAngle)/(dissolveMaxAngle-dissolveMinAngle);
        offset = Mathf.Clamp01(offset);

        if(Mathf.Sign(angleOffset)>0)
            expandCircle.circleRadius = Mathf.Lerp(initRadius, dissolveRadius, EasingFunc.Easing.QuadEaseIn(offset));
        else
            expandCircle.circleRadius = Mathf.Lerp(initRadius, shrinkRadius, EasingFunc.Easing.QuadEaseIn(offset));

        if(offset>=1){
            iC_Stylized.StartExpand();
        }
    }
}
