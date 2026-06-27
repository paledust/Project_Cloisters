using UnityEditor;
using UnityEngine;

namespace SimpleAudioSystem.Edit
{
    [CustomEditor(typeof(AudioDataCollection_SO))]
    public class AudioDataCollectionInspector : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if(GUILayout.Button("Find All Data"))
            {
                var collection = target as AudioDataCollection_SO;
                var guids = AssetDatabase.FindAssets("t:AudioData_SO");
                bool dirtyFlag = false;
                foreach(string guid in guids)
                {
                    string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                    AudioData_SO data = AssetDatabase.LoadAssetAtPath<AudioData_SO>(assetPath);
                    if(data.name.Contains("bgm"))
                    {
                        if(!collection.bgm_info_list.Contains(data))
                        {
                            dirtyFlag = true;
                            collection.bgm_info_list.Add(data);
                        }
                    }
                    else if(data.name.Contains("amb"))
                    {
                        if(!collection.amb_info_list.Contains(data))
                        {
                            dirtyFlag = true;
                            collection.amb_info_list.Add(data);
                        }
                    }
                    else if(!collection.sfx_info_list.Contains(data))
                    {
                        dirtyFlag = true;
                        collection.sfx_info_list.Add(data);
                    }
                }
                if(dirtyFlag)
                    EditorUtility.SetDirty(collection);
            }
        }
    }
}