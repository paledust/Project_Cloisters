using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("InteractionController_Manager")]
public class IC_Manager : MonoBehaviour
{
    [SerializeField] private IC_Basic[] interactionControllers;
    void Awake(){
        
    }
    void OnDestroy(){
        
    }
}
