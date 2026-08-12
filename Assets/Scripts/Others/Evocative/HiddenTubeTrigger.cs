using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenTubeTrigger : MonoBehaviour
{
    [SerializeField] private HiddenTube tube;
    [SerializeField] private bool isPathReverse;
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent<BounceBall>(out var bounceBall))
        {
            tube.TryStartTravelling(bounceBall, isPathReverse);
        }
    }
}
