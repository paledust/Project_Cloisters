using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using UnityEngine;

[RequireComponent(typeof(ConstantForce))]
public class RotationSpring : MonoBehaviour
{
    private enum SpringState
    {
        Free,
        Spring,
        Locked
    }
    [Header("Angle Correction")]
    [SerializeField] private float checkAngle = 45;
    [SerializeField] private float correctForce = 10;
    [SerializeField] private float minAngularSpeed = 3;
    [SerializeField] private float stopAngularSpeed = 0.1f;
    [Header("TorqueBoost")]
    [SerializeField] private float collisionBoost = 1;
    [SerializeField, ShowOnly] private SpringState springState = SpringState.Locked;
    [SerializeField, ShowOnly] private float targetAngle;
    private float[] angleArray;
    private Rigidbody rigid;
    private ConstantForce constForce;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        constForce = GetComponent<ConstantForce>();
        springState = SpringState.Locked;

        angleArray = new float[4];
        angleArray[0] = checkAngle;
        angleArray[1] = 180 - checkAngle;
        angleArray[2] = 180 + checkAngle;
        angleArray[3] = 360 - checkAngle;

        targetAngle = GetTargetAngle();
    }

    void Update()
    {
        switch (springState)
        {
            case SpringState.Free:
                if (rigid.angularVelocity.magnitude < minAngularSpeed)
                {
                    targetAngle = GetTargetAngle();
                    springState = SpringState.Spring;
                }
                break;
            case SpringState.Spring:
                float currentAngle = rigid.rotation.eulerAngles.z;
                if (currentAngle < 0)
                {
                    currentAngle += 360;
                }
                if (Mathf.Abs(currentAngle-targetAngle) < 0.1f && rigid.angularVelocity.magnitude < stopAngularSpeed)
                {
                    springState = SpringState.Locked;
                    rigid.angularVelocity = Vector3.zero;
                    rigid.rotation = Quaternion.Euler(0, 0, targetAngle);
                    constForce.torque = Vector3.zero;
                }
                else
                {
                    float force = (targetAngle - currentAngle) * Mathf.Deg2Rad * correctForce;
                    force = Mathf.Clamp(force, -2*correctForce, 2*correctForce);
                    constForce.torque = Vector3.forward * force;
                }
                break;
        }
    }

    float GetTargetAngle()
    {
        float idealAngle = angleArray[0];
        float currentAngle = rigid.rotation.eulerAngles.z;
        if (currentAngle < 0)
        {
            currentAngle += 360;
        }
        for (int i = 0; i < angleArray.Length; i++)
        {
            if (Mathf.Abs(idealAngle - currentAngle) > Mathf.Abs(angleArray[i] - currentAngle))
            {
                idealAngle = angleArray[i];
            }
        }
        return idealAngle;
    }

    void OnCollisionEnter(Collision collision)
    {
        springState = SpringState.Free;
        constForce.torque = Vector3.zero;
        rigid.AddForceAtPosition(collision.impulse*collisionBoost, collision.GetContact(0).point, ForceMode.VelocityChange);
    }
}
