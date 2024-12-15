using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct CurveData
{
    public AnimationCurve curve;
    public float scaler;
    public float Evaluate(float t)=>scaler*(curve==null?1:curve.Evaluate(t));
}
