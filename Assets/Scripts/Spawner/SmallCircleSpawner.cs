using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCircleSpawner : Basic_ObjectPool<CollidableCircle>
{
    [SerializeField] private RectSelector rectSelector;
    [SerializeField] private Vector2 spawnCycleRange;
    [SerializeField] private Vector2Int spawnAmountRange;
[Header("Spawn Settings")]
    [SerializeField] private Vector2 spawnSize;

    public static RectSelector m_rectSelector;
    
    private float currentCycle;
    private float spanwTimer;

    protected override void Awake(){
        base.Awake();
        m_rectSelector = rectSelector;
        currentCycle = spawnCycleRange.GetRndValueInVector2Range();
        spanwTimer = Time.time-currentCycle;
    }
    protected override void PrepareTarget(CollidableCircle target)
    {
        base.PrepareTarget(target);
        target.GetComponent<Clickable_Circle>().DisableHitbox();
        target.ResetGrowingAndWobble();
        target.ResetMotion();
        target.ResetSize(spawnSize.GetRndValueInVector2Range());
        target.transform.position = rectSelector.GetPoint();
    }
    void Update(){
        if(Time.time-spanwTimer>=currentCycle){
            spanwTimer = Time.time;
            currentCycle = spawnCycleRange.GetRndValueInVector2Range();
            int amount = spawnAmountRange.GetRndValueInVector2Range();
            for(int i=0;i<amount;i++){
                var go = GetObjFromPool(x=>!x.gameObject.activeSelf);
                if(go!=null) {
                    go.gameObject.SetActive(true);
                    go.FloatUp(Random.Range(4f, 5f));
                }
            }
        }

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