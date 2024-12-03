using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeCircleSpawner : Basic_ObjectPool<CollidableCircle>
{
    public CollidableCircle SpawnCircle(Transform spawnTrans){
        var go = GetObjFromPool(x=>!x.gameObject.activeSelf);
        if(go == null) return null;

        Vector3 dir = (Vector3)Random.insideUnitCircle.normalized;
        go.transform.position = spawnTrans.position + dir*0.2f;
        go.GetComponent<Rigidbody>().AddForce(dir*3);
        return GetObjFromPool(x=>!x.gameObject.activeSelf);
    }
    protected override void PrepareTarget(CollidableCircle target)
    {
        base.PrepareTarget(target);
        target.GetComponent<Clickable_Circle>().DisableHitbox();
        StartCoroutine(CommonCoroutine.delayAction(()=>{
            target.GetComponent<Clickable_Circle>().EnableHitbox();
        }, 0.5f));
    }
}
