using UnityEngine;

[System.Serializable]
public struct NarrativeRect
{
    public Transform centerTrans;
    [SerializeField] private Rect rect;
    [Header("Soft Padding")]
    [SerializeField] private Vector2 softPadding;
    [SerializeField] private float crossBoundPushVelThreashold;
    [SerializeField] private float forceApply;
    public Vector3 GetCrossBoundForce(Rigidbody m_rigid)
    {
        Vector3 pos = m_rigid.position;
        Vector3 force = Vector2.zero;
        Vector3 centerDir = (centerTrans.position - pos).normalized;
        //If inside soft padding area, no force applied
        if(pos.x > MinX+softPadding.x*0.5f && pos.x < MaxX-softPadding.x*0.5f &&
           pos.y > MinY+softPadding.y*0.5f && pos.y < MaxY-softPadding.y*0.5f)
        {
            return force;
        }
        if(pos.x < MinX + softPadding.x*0.5f ||
           pos.x > MaxX - softPadding.x*0.5f)
        {
            if(Vector3.Dot(m_rigid.velocity, centerDir)<crossBoundPushVelThreashold)
                force -= Vector3.right * (pos.x - centerTrans.position.x< 0 ? 1 : -1) * forceApply;
        }
        if(pos.y < MinY + softPadding.y*0.5f ||
           pos.y > MaxY - softPadding.y*0.5f)
        {
            if(Vector3.Dot(m_rigid.velocity, centerDir)<crossBoundPushVelThreashold)
                force -= Vector3.up * (pos.y - centerTrans.position.y < 0 ? 1 : -1) * forceApply;
        }
        return force;
    }
    public float MinX => centerTrans.position.x + rect.xMin - rectWidth * 0.5f;
    public float MaxX => centerTrans.position.x + rect.xMax - rectWidth * 0.5f;
    public float MinY => centerTrans.position.y + rect.yMin - rectHeight * 0.5f;
    public float MaxY => centerTrans.position.y + rect.yMax - rectHeight * 0.5f;
    public float rectWidth => rect.width;
    public float rectHeight => rect.height;
#if UNITY_EDITOR
    public void DrawGizmo(Color color){
        if(centerTrans==null) return;
        Gizmos.color = color;
        Gizmos.DrawCube(centerTrans.position + new Vector3(rect.x, rect.y), new Vector3(rect.width, rect.height, 0.01f));
        Gizmos.color = new Color(1, 0.2f, 0.2f, 0.2f);
        Gizmos.DrawCube(centerTrans.position + new Vector3(rect.x, rect.y), new Vector3(rect.width - softPadding.x, rect.height - softPadding.y, 0.01f));
    }
#endif
}