using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class PlanetCameraController : MonoBehaviour
{
    [SerializeField] private CinemachineMixingCamera mixingCamera;
[Header("Blend")]
    [SerializeField] private float maxBlend;
    [SerializeField] private float blendStartAngle;
    [SerializeField] private float blendLength;
    [SerializeField] private AnimationCurve blendCurve;
    [SerializeField] private bool inverseBlend = false;
[Header("Planet")]
    [SerializeField] private Transform surroundPlanet;
    [SerializeField] private Transform centerPlanet;

    void Update()
    {
        Vector3 diff = surroundPlanet.position - centerPlanet.position;
        diff.y = 0;
        float angle = Vector3.SignedAngle(diff, Vector3.back, Vector3.up);
        angle = Mathf.Abs(angle);

        float targetRatio = Mathf.Clamp01((angle - blendStartAngle)/blendLength);
        if(inverseBlend) targetRatio = 1-targetRatio;
        targetRatio = blendCurve.Evaluate(targetRatio);

        mixingCamera.m_Weight0 = Mathf.Lerp(1, 1-maxBlend, targetRatio);
        mixingCamera.m_Weight1 = Mathf.Lerp(0, maxBlend, targetRatio);
    }
}