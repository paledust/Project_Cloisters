using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeEntranceTrigger : MonoBehaviour
{
    private EvocativeTube tube;
    private Collider trigger;

    void Awake()
    {
        trigger = GetComponent<Collider>();
    }
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BounceBall ball))
        {
            tube.EnterTube(ball);
            trigger.enabled = false;
        }
    }

    public void InitTrigger(EvocativeTube tube)
    {
        this.tube = tube;
        trigger.enabled = true;
    }
    public void ResetTubeTrigger() => trigger.enabled = true;
}
