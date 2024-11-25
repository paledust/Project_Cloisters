using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Initiator : MonoBehaviour
{
    [SerializeField] private IC_Manager interController_Manager;
[Header("Debug Option")]
    [SerializeField] private int StartIndex = 0;
    void Start()
    {
    #if UNITY_EDITOR
        interController_Manager.StartAtInteraction(StartIndex);
    #else
        interController_Manager.StartInteraction(0);
    #endif
    }

#if UNITY_EDITOR
    [ContextMenu("Set Up Scene To Interactions")]
    public void Editor_SetUpInteractions(){
        interController_Manager.CleanUpAllInteractions();
        interController_Manager.Editor_ActivateInteractions(StartIndex);
    }
#endif
}
