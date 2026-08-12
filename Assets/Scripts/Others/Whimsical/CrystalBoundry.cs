using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class CrystalBoundry : MonoBehaviour
{
    [SerializeField] private float pushStrength = 1;
    [SerializeField] private float pushDuration = 1;
    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<Clickable_Crystal>(out var crystal))
        {
            Vector3 offset = transform.right * Vector3.Dot(transform.right,other.transform.position - transform.position);
            Vector3 pushPos = transform.position + transform.up * (transform.localScale.y * 0.5f + Random.Range(1,2) * pushStrength);
            pushPos.z = crystal.transform.position.z;
            
            crystal.PushTowardPos(pushPos + offset * Random.Range(.8f, 1), pushDuration);
        }
    }
}
