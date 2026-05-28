#if UNITY_EDITOR
using UnityEditor;
#endif

using UnityEngine;

public class RotateAround : MonoBehaviour
{
    public enum RotateAxis
    {
        Foward = 0,
        Up = 1,
        Right = 2,
    }
    [SerializeField] private Transform targetTrans;
    [SerializeField, ShowOnly] private float rotateAngle;
    [SerializeField] private RotateAxis rotateAxis = RotateAxis.Foward;
    public Vector3 axisAdjust;
    public float angularSpeed = 10;

    public float m_rotateAngle => rotateAngle;
    
    private Vector3 initDiff;
    private Vector3 axis;

    // Start is called before the first frame update
    void Start()
    {
        InitializeRotateParam();
    }
    void FixedUpdate(){
        StepSim(angularSpeed * Time.fixedDeltaTime);
    }
    void InitializeRotateParam(){
        Vector3 diff = transform.position - targetTrans.position;
        initDiff = diff;
        rotateAngle = 0;

        Vector3 targetAxis = targetTrans.forward;
        switch(rotateAxis)
        {
            case RotateAxis.Foward:
                targetAxis = targetTrans.forward;
                break;
            case RotateAxis.Up:
                targetAxis = targetTrans.up;
                break;
            case RotateAxis.Right:
                targetAxis = targetTrans.right;
                break;
        }
        axis = Vector3.Cross(targetAxis, diff).normalized;
    }
    public void StepSim(float angleStep)
    {
        rotateAngle += angleStep;
        if(rotateAngle>=360) rotateAngle = 0;
        if(rotateAngle<=-360) rotateAngle = 0;

        transform.position = targetTrans.position + Quaternion.AngleAxis(rotateAngle, Quaternion.Euler(axisAdjust)*axis)*initDiff;
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
