using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeSpawner : Basic_ObjectPool<NarrativeText>
{
    [SerializeField] private RectSelector rectSelector;
    protected override void PrepareTarget(NarrativeText target)
    {
        base.PrepareTarget(target);
        target.transform.position = rectSelector.GetPoint();
        target.transform.parent = rectSelector.centerTrans;
    }
    void Update(){
        foreach(var n_text in pools){
            if(!n_text.gameObject.activeSelf){
                RecycleTarget(n_text);
            }
        }
    }
    public void PlaceText(){
        var go = GetObjFromPool(x=>!x.gameObject.activeSelf);
        if(go!=null){
            go.gameObject.SetActive(true);
            go.FadeInText();
        }
    }
    void OnDrawGizmosSelected(){
        rectSelector.DrawGizmo(new Color(0,1,0,0.25f));
    }
}
