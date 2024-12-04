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
    [SerializeField] private Vector2 floatUpForceRange;
    private float currentCycle;
    private float spanwTimer;
    protected override void Awake(){
        base.Awake();
        currentCycle = spawnCycleRange.GetRndValueInVector2Range();
        spanwTimer = Time.time-currentCycle;
    }
    protected override void PrepareTarget(CollidableCircle target)
    {
        base.PrepareTarget(target);
        target.GetComponent<Clickable_Circle>().DisableHitbox();
        target.ResetGrowingAndWobble();
        target.ResetMotion(floatUpForceRange.GetRndValueInVector2Range());
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
                if(go!=null) go.gameObject.SetActive(true);
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