using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
    [SerializeField] private Transform targetTrans;
    public Vector3 axisAdjust;
    public float angularSpeed = 10;

    public Transform m_target{get{return targetTrans;}}
    
    private float rotateAngle;
    private Vector3 initDiff;
    private Vector3 axis;

    // Start is called before the first frame update
    void Start()
    {
        InitializeRotateParam();
    }

    void FixedUpdate(){
        rotateAngle += angularSpeed * Time.fixedDeltaTime;
        if(rotateAngle>=360) rotateAngle = 0;
        if(rotateAngle<=-360) rotateAngle = 0;

        transform.position = targetTrans.position + Quaternion.AngleAxis(rotateAngle, Quaternion.Euler(axisAdjust)*axis)*initDiff;
    }
    void InitializeRotateParam(){
        Vector3 diff = transform.position - targetTrans.position;
        initDiff = diff;
        rotateAngle = 0;
        axis = Vector3.Cross(targetTrans.forward, diff).normalized;        
    }
#if UNITY_EDITOR
    void OnDrawGizmos(){
        if(!EditorApplication.isPlaying){
            InitializeRotateParam();
        }
        DebugExtension.DrawCircle(targetTrans.position, Quaternion.Euler(axisAdjust)*axis, Color.yellow, initDiff.magnitude);
    }
#endif
}
