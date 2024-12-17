using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class EclipseDeform
{
    public static float DeformedRadius(float circleRadius, float eclipseValue, float rad){
        float cos = Mathf.Cos(rad);
        float sin = Mathf.Sin(rad);
        float e2 = eclipseValue*eclipseValue;
        if(eclipseValue>1){
            Debug.LogAssertion("Eclipse Value bigger than 1(hyperbola), return 0");
            return 0;
        }
        return circleRadius * Mathf.Sqrt(cos*cos+e2*sin*sin);
    }
}