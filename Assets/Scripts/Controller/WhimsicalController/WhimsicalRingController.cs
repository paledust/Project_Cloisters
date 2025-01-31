using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhimsicalRingController : MonoBehaviour
{
    [SerializeField] private Clickable_ObjectRotator crystal;
    [SerializeField] private List<CrystalRing> rings;
[Header("Expand")]
    [SerializeField] private AnimationCurve opacityCurve;
    [SerializeField] private Vector2 ringScaleRange;
    [SerializeField] private float speedToExpand = 1;
[Header("Recycle")]
    [SerializeField] private Vector2 rollRange;
    [SerializeField] private Vector2 pitchRange;

    void Update()
    {
        foreach(var ring in rings)
        {
            ring.transform.localScale += Vector3.one * speedToExpand * crystal.m_angularSpeed * Time.deltaTime;

            if(ring.transform.localScale.x > 2*ringScaleRange.y-ringScaleRange.x)
            {
                ring.RefreshParticle();
                ring.transform.localScale = ringScaleRange.x * Vector3.one;
                ring.transform.rotation = Quaternion.Euler(GetRndAngle(pitchRange),0,0) * Quaternion.Euler(0,0,GetRndAngle(rollRange));
            }
            else if(ring.transform.localScale.x < ringScaleRange.x)
            {
                ring.RefreshParticle();
                ring.transform.localScale = (2*ringScaleRange.y-ringScaleRange.x) * Vector3.one;
                ring.transform.rotation = Quaternion.Euler(GetRndAngle(pitchRange),0,0) * Quaternion.Euler(0,0,GetRndAngle(rollRange));
            }
            ring.UpdateRingColor(opacityCurve.Evaluate(Mathf.InverseLerp(ringScaleRange.x, ringScaleRange.y, ring.transform.localScale.x)));
        }
    }
    float GetRndAngle(Vector2 range)
    {
        float angle = range.GetRndValueInVector2Range();
        return (Random.value>0.5f?1:-1) * angle;
    }
}
