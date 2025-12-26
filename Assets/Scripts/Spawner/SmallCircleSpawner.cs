using UnityEngine;

public class SmallCircleSpawner : Basic_ObjectPool<CollidableCircle>
{
[Header("Spawn")]
    [SerializeField] private RectSelector rectSelector;
[Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnSize;
    [SerializeField] private SphereCollider heroCircleCollider;

    public static RectSelector m_rectSelector;

    protected override void Awake(){
        base.Awake();
        m_rectSelector = rectSelector;
    }
    protected override void PrepareTarget(CollidableCircle target)
    {
        base.PrepareTarget(target);
        target.GetComponent<Clickable_Circle>().DisableHitbox();
        target.ResetGrowingAndWobble();
        target.ResetMotion();
        target.ResetSize(spawnSize.GetRndValueInVector2Range());
        target.transform.position = rectSelector.GetPoint();
        if (heroCircleCollider != null)
        {
            Vector2 diff = target.transform.position - heroCircleCollider.transform.position;
            float minDist = heroCircleCollider.radius + target.radius;
            if(diff.sqrMagnitude < minDist * minDist)
            {
                // Inside hero circle, move outside
                diff = diff.normalized * (minDist+2);
                target.transform.position = (Vector2)heroCircleCollider.transform.position + diff;
            }
        }
    }
    public void SpawnAtPoint(Vector3 point)
    {
        var go = GetObjFromPool(x=>!x.gameObject.activeSelf);
        if(go!=null) {
            go.transform.position = point;
            go.gameObject.SetActive(true);
            go.FloatUp(Random.Range(4f, 5f));
        }
    }
    void Update(){
        for(int i=0; i<pools.Count; i++){
            if(pools[i].Collidable && !pools[i].IsVisible)
            {
                pools[i].gameObject.SetActive(false);
                RecycleTarget(pools[i]);
            }
        }
    }
    void OnDrawGizmosSelected(){
        rectSelector.DrawGizmo(new Color(0,1,0,0.25f));
    }
}