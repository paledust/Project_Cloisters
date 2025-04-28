using UnityEngine;

public class Clickable_ConnectionBreaker : Basic_Clickable
{
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
        BreakConnection(breakForce, breakTorque, hitPos);
    }
    public void BreakConnection(float force, float torque, Vector3 breakPos)
    {
        Destroy(connectJoint);
        main.m_connectBody.BreakConnection(other.m_connectBody);
        other.m_connectBody.BreakConnection(main.m_connectBody);
        main.m_connectBody.m_rigid.AddForce(-main.transform.up*force, ForceMode.Impulse);
        main.m_connectBody.m_rigid.AddTorque(Vector3.forward*torque, ForceMode.Impulse);
        other.m_connectBody.m_rigid.AddForce(-other.transform.up*force, ForceMode.Impulse);
        other.m_connectBody.m_rigid.AddTorque(-Vector3.forward*torque, ForceMode.Impulse);
        main.OnConnectionBreak();
        other.OnConnectionBreak();
        Destroy(gameObject);
        EventHandler.Call_OnBreakConnectionBreaker(this, breakPos);  
    }
}