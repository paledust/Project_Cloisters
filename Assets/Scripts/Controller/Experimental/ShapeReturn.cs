using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeReturn : MonoBehaviour
{
    [SerializeField] private Vector2 shapeValidRectSize;
    private Rect shapeValidRect;
    private ConnectBody[] allBodies;

    public void Init(ConnectBody[] connectBodies)
    {
        shapeValidRect = new Rect(-shapeValidRectSize / 2, shapeValidRectSize);
        allBodies = connectBodies;
    }
    void Update()
    {
        foreach (var body in allBodies)
        {
            if(!body.gameObject.activeSelf)
                continue;
            
            Vector3 pos = body.transform.localPosition;
            if(shapeValidRect.Contains(pos))
            {
                
            }
        }
    }
}
