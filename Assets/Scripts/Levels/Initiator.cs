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
        interController_Manager.StartInteraction(StartIndex);
    #else
        interController_Manager.StartInteraction(0);
    #endif
    }
}
