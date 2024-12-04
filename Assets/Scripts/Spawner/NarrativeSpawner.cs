using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NarrativeSpawner : Basic_ObjectPool<NarrativeText>
{
    [SerializeField] private RectSelector rectSelector;
    void OnDrawGizmosSelected(){
        rectSelector.DrawGizmo(new Color(0,1,0,0.25f));
    }
}
