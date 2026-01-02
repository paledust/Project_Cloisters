using UnityEngine;

[System.Serializable]
public struct RectSelector
{
    public Transform centerTrans;
    [SerializeField] private Rect rect;
    public Vector3 GetPoint(){
        return centerTrans.position + Vector3.right * Random.Range(-rect.width, rect.width)*0.5f + Vector3.up * Random.Range(-rect.height, rect.height)*0.5f;
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
    }
#endif
}