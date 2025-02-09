using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhimsicalShadeControl : MonoBehaviour
{
    [SerializeField] private float phase;
    [SerializeField] private Transform crystal;
    [SerializeField] private float speedToPhaseSpeed;
    [SerializeField] private Gradient shadeGradient;
    [SerializeField] private float proxySize;
    [SerializeField] private SpriteRenderer[] shades;
    void Update()
    {
        phase = crystal.localEulerAngles.y/360*speedToPhaseSpeed;

        for(int i=0; i<shades.Length; i++)
        {
            shades[i].color = shadeGradient.Evaluate(Service.Fract(phase + i*0.25f));
        }
    }
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(crystal.transform.position, Vector3.one*proxySize);
    }
}
