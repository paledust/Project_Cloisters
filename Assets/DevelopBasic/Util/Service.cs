using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Service{
    public static int InteractableLayer = LayerMask.NameToLayer("Interactable");
    public static int IgnoreRaycastLayer = LayerMask.NameToLayer("Ignore Raycast");
#region HelperFunction
    public static float Fract(float value)=>value-Mathf.Floor(value);
    public static T[] FindComponentsOfTypeIncludingDisable<T>(){
        int sceneCount = UnityEngine.SceneManagement.SceneManager.sceneCount;
        var MatchObjects = new List<T> ();

        for(int i=0; i<sceneCount; i++){
            var scene = UnityEngine.SceneManagement.SceneManager.GetSceneAt (i);
            
            var RootObjects = scene.GetRootGameObjects ();

            foreach (var obj in RootObjects) {
                var Matches = obj.GetComponentsInChildren<T> (true);
                MatchObjects.AddRange (Matches);
            }
        }

        return MatchObjects.ToArray ();
    }
    public static void Shuffle<T>(ref T[] elements){
        var rnd = new System.Random();
        for(int i=0; i<elements.Length; i++){
            int index = rnd.Next(i+1);
            T tmp = elements[i];
            elements[i] = elements[index];
            elements[index] = tmp;
        }
    }
    public static void Shuffle<T>(ref List<T> elements){
        var rnd = new System.Random();
        for(int i=0; i<elements.Count; i++){
            int index = rnd.Next(i+1);
            T tmp = elements[i];
            elements[i] = elements[index];
            elements[index] = tmp;
        }
    }
#endregion
}