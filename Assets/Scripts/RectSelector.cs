using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct RectSelector
{
    [SerializeField] private Transform centerTrans;
    [SerializeField] private Rect rect;
    public Vector3 GetPoint(){
        return centerTrans.position + Vector3.right * Random.Range(-rect.width, rect.width)*0.5f + Vector3.up * Random.Range(-rect.height, rect.height)*0.5f;
    }
#if UNITY_EDITOR
    public void DrawGizmo(Color color){
        Gizmos.color = color;
        Gizmos.DrawCube(centerTrans.position + new Vector3(rect.x, rect.y), new Vector3(rect.width, rect.height, 0.01f));        
    }
#endif
}