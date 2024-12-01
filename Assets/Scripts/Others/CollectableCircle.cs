using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCircle : MonoBehaviour
{
    [SerializeField] private PerRendererDissolve[] circleRender;
    void OnCollisionEnter(Collision other){
        if(other.gameObject.layer == Service.InteractableLayer){
        }
    }
}
