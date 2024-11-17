using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAroundControl : MonoBehaviour
{
    [SerializeField] private RotateAround rotateAround;
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private float controlAgility = 5;
[Header("Speed Remapping")]
    [SerializeField] private float speedScale;
    [SerializeField] private float speedOffset;

    void Update()
    {
        rotateAround.angularSpeed = Mathf.Lerp(rotateAround.angularSpeed, clickable_Planet.m_angularSpeed*speedScale - speedOffset, Time.deltaTime * controlAgility);
    }
}
