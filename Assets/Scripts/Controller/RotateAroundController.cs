using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundController : MonoBehaviour
{
    [SerializeField] private RotateAround rotateAround;
    [SerializeField] private Clickable_ObjectRotator clickable_Planet;
    [SerializeField] private float controlAgility = 5;
[Header("Speed Remapping")]
    [SerializeField] private float speedScale;
    [SerializeField] private float speedOffset;

    void Update()
    {
        rotateAround.angularSpeed = Mathf.Lerp(rotateAround.angularSpeed, clickable_Planet.m_angularSpeed*speedScale - (clickable_Planet.m_isControlling?0:speedOffset), Time.deltaTime * controlAgility);

        Vector3 euler = rotateAround.m_target.eulerAngles;
        if(euler.x > 180) euler.x -= 360;
        euler *= 0.5f;
        rotateAround.axisAdjust = Vector3.Lerp(rotateAround.axisAdjust, euler, Time.deltaTime * controlAgility);
    }
}
