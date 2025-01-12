using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorFlip : MonoBehaviour
{
    [SerializeField] private Transform mirrorTrans;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Dot(Vector3.forward, mirrorTrans.forward)>0){
            mirrorTrans.localRotation *= Quaternion.Euler(180,0,0);

        }
    }
}
