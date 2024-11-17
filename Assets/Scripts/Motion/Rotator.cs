using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotator : MonoBehaviour
{
    public Vector3 axis = Vector3.up;
    public float angularSpeed = 90;
    void Update()
    {
        transform.Rotate(axis,angularSpeed*Time.deltaTime,Space.Self);
    }
}
