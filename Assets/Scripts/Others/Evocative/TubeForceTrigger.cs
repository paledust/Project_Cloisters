using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TubeForceTrigger : MonoBehaviour
{
    [SerializeField] private float forceScale = 24;
    [SerializeField] private ParticleSystem vfxAttraction;
    private Rigidbody ballRigid;
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out BounceBall ball))
        {
            ballRigid = ball.GetComponent<Rigidbody>();
            vfxAttraction.Play();
        }
    }
    void OnTriggerExit(Collider other)
    {
        if(other.TryGetComponent(out BounceBall ball))
        {
            ballRigid = null;
            vfxAttraction.Stop();
        }
    }
    void FixedUpdate()
    {
        if(ballRigid != null)
        {
            Vector2 pos = ballRigid.position;
            Vector2 localPos = transform.InverseTransformPoint(pos);
            ballRigid.AddForce(forceScale * transform.up);
        }
    }
}
