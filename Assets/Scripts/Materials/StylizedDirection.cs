using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class StylizedDirection : MonoBehaviour
{
    private static readonly int StylizedDirID = Shader.PropertyToID("StylizedDir");
    void Start()
    {
        Shader.SetGlobalVector(StylizedDirID, transform.forward);
    }
    void Update()
    {
        Shader.SetGlobalVector(StylizedDirID, transform.forward);
    }
    void OnDrawGizmos(){
        DebugExtension.DrawArrow(transform.position, transform.forward, Color.blue);
    }
}
