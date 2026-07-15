using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RAMRelease
{
    [MenuItem("Tools/Release RAM")]
    public static void Release()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
        System.GC.Collect();
    }
}
