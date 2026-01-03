using System.Collections.Generic;
using UnityEngine;

public class NarrativeCircleManager : Basic_ObjectPool<CollidableCircle>
{
    public enum SpawnStyle
    {
        FloatUp,
        PopUp
    }
[Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnSize;
[Header("Border Setting")]
    [SerializeField] private RectSelector rectSelector;
[Header("Force Field")]
    [SerializeField] private NarrativeRandomForceField forceField;

    private List<CollidableCircle> listCircles = new List<CollidableCircle>();
    private IC_Narrative narrativeController;

    protected override void Awake(){
        base.Awake();
        narrativeController = GetComponent<IC_Narrative>();
    }
    void LateUpdate()
    {
        for(int i = listCircles.Count - 1; i>=0; i--)
        {
            var circle = listCircles[i];
            if(circle.isPined)
            {
                if(!circle.m_circle.isControlling)
                {
                    forceField.ApplyForce(circle.m_rigidbody, 5);
                }
                continue;
            }
            if(!circle.m_circle.isControlling)
            {
                forceField.ApplyForce(circle.m_rigidbody, 1);
            }
            Vector3 pos = circle.transform.position;
            if(circle.transform.position.x < rectSelector.MinX)
            {
                pos.x += rectSelector.rectWidth;
            }
            else if(circle.transform.position.x > rectSelector.MaxX)
            {
                pos.x -= rectSelector.rectWidth;
            }

            if(circle.transform.position.y < rectSelector.MinY)
            {
                pos.y += rectSelector.rectHeight;
            }
            else if(circle.transform.position.y > rectSelector.MaxY)
            {
                pos.y -= rectSelector.rectHeight;
            }

            circle.transform.position = pos;
            circle.m_rigidbody.position = pos;
        }
    }
    public CollidableCircle SpawnAtPoint(Vector3 point, float duration, SpawnStyle style)
    {
        var go = GetObjFromPool(x=>!x.gameObject.activeSelf);
        if(go!=null) {
            go.transform.position = point;
            go.gameObject.SetActive(true);
            switch(style)
            {
                case SpawnStyle.PopUp:
                    go.PopUp(duration);
                    break;
                case SpawnStyle.FloatUp:
                    go.FloatUp(duration);
                    break;
            }
        }
        listCircles.Add(go);
        return go;
    }
    protected override void PrepareTarget(CollidableCircle target)
    {
        base.PrepareTarget(target);
        target.GetComponent<Clickable_Circle>().DisableHitbox();
        target.ResetGrowingAndWobble();
        target.ResetMotion();
        target.ResetSize(spawnSize.GetRndValueInVector2Range());
        target.m_circle.ChangeCircleType(Clickable_Circle.CircleType.Normal);
    }
    void OnDrawGizmosSelected()
    {
        rectSelector.DrawGizmo(new Color(0,1,0,0.25f));
    }
}