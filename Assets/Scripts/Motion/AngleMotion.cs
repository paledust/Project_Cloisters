using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AngleMotion : MonoBehaviour
{
    [SerializeField] private float maxAngle;
    [SerializeField] private float freq;
    [SerializeField] private Vector3 axis = Vector3.forward;
    [SerializeField] private bool snappyRotate = false;
    [SerializeField] private float snappyStep = 1;
    [Range(0, 1)]
    public float ControlFactor;

    private float timer = 0;
    private float seed;
    private Quaternion initRot;

    void OnEnable()
    {
        seed = Random.Range(-1f, 1f);
        initRot = transform.localRotation;
    }

    void Update()
    {
        timer += Time.deltaTime*freq;
        if(snappyRotate){
            float angle = Mathf.Sin((timer + seed)*Mathf.PI) * maxAngle;
            angle = Mathf.Floor(angle*snappyStep)/snappyStep;
            transform.localRotation = Quaternion.Euler(axis * angle)*initRot;
        }
        else{
            transform.localRotation = Quaternion.Euler(ControlFactor * axis * Mathf.Sin((timer + seed)*Mathf.PI) * maxAngle)*initRot;
        }
    }
}
