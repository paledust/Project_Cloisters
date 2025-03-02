using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_ConnectionBreaker : Basic_Clickable
{
    [SerializeField] private SpriteRenderer breakerConnector;
    [SerializeField] private ParticleSystem connectParticle;
    [SerializeField] private ParticleSystem breakParticle;
    [SerializeField] private float breakForce = 20f;
    [SerializeField] private float breakTorque = 30f;
    private ConnectTrigger main;
    private ConnectTrigger other;
    private Joint connectJoint;
    public void InitConnection(Joint joint, ConnectTrigger main, ConnectTrigger other)
    {
        this.main = main;
        this.other = other;
        this.connectJoint = joint;
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        Destroy(connectJoint);
        main.m_connectBody.BreakConnection(other.m_connectBody);
        other.m_connectBody.BreakConnection(main.m_connectBody);
        main.m_connectBody.m_rigid.AddForce(-main.transform.up*breakForce, ForceMode.Impulse);
        main.m_connectBody.m_rigid.AddTorque(Vector3.forward*breakTorque, ForceMode.Impulse);
        other.m_connectBody.m_rigid.AddForce(-other.transform.up*breakForce, ForceMode.Impulse);
        other.m_connectBody.m_rigid.AddTorque(-Vector3.forward*breakTorque, ForceMode.Impulse);
        main.SwitchTrigger(true);
        other.SwitchTrigger(true);
        Destroy(gameObject);
    }
}