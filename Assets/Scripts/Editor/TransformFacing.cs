using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UIElements;

public class TransformFacing : EditorWindow
{
    Transform center;
    [MenuItem("Tools/Transform/TransformFacing")]
    public static void ShowWindow()=>GetWindow<TransformFacing>();
    void OnGUI()
    {
        center = EditorGUILayout.ObjectField(center, typeof(Transform), true) as Transform;

        if(GUILayout.Button("Align Facing"))
        {
            var selects = Selection.gameObjects;
            Undo.RecordObjects(selects, "Change Facing Direction");
            foreach(var go in selects)
            {
                go.transform.LookAt(center);
                EditorUtility.SetDirty(go);
            }
        }

    }
}
