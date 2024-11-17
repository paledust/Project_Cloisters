using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private Transform targetTrans;
    [SerializeField] private Vector3 rotatePlaneTangent;
    public float angularSpeed = 10;
    private float radius;
    private Vector3 axis;
    // Start is called before the first frame update
    void Start()
    {
        InitializeRotateParam();
    }

    void FixedUpdate(){
        float dt = Time.fixedDeltaTime;
        Vector3 diff = transform.position - targetTrans.position;
        transform.position += Vector3.Cross(axis, diff).normalized*angularSpeed*dt - diff.normalized*(angularSpeed*angularSpeed)/(2*radius)*dt*dt;
    }
    void InitializeRotateParam(){
        Vector3 diff = transform.position - targetTrans.position;
        radius = diff.magnitude;
        axis = Vector3.Cross(rotatePlaneTangent, diff).normalized;        
    }
#if UNITY_EDITOR
    void OnDrawGizmosSelected(){
        if(!EditorApplication.isPlaying){
            InitializeRotateParam();
        }
        DebugExtension.DrawCircle(targetTrans.position, axis, Color.yellow, radius);
    }
#endif
}
