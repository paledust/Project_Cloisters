using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallCircleSpawner : Basic_ObjectPool<CollidableCircle>
{
    [SerializeField] private Transform spawnStart;
    [SerializeField] private float SpawnRadius;
    [SerializeField] private float maxOffset;
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
        target.ResetFloat(floatUpForceRange.GetRndValueInVector2Range());
        target.ResetSize(spawnSize.GetRndValueInVector2Range());
        target.transform.position = spawnStart.position + Vector3.right * Random.Range(-SpawnRadius, SpawnRadius) * 0.5f;
        target.gameObject.SetActive(true);
    }
    void Update(){
        if(Time.time-spanwTimer>=currentCycle){
            spanwTimer = Time.time;
            currentCycle = spawnCycleRange.GetRndValueInVector2Range();
            int amount = spawnAmountRange.GetRndValueInVector2Range();
            for(int i=0;i<amount;i++){
                GetObjFromPool(x=>!x.gameObject.activeSelf);
            }
        }
        for(int i=0; i<pools.Count; i++){
            if(pools[i].Collidable && !pools[i].IsVisible)
            {
                pools[i].gameObject.SetActive(false);
            }
        }
    }
    void OnDrawGizmosSelected(){
        Gizmos.color = new Color(0,1,0,0.25f);
        Gizmos.DrawCube(spawnStart.position + Vector3.up * maxOffset * 0.5f, new Vector3(SpawnRadius, maxOffset, 0.01f));
    }
}
