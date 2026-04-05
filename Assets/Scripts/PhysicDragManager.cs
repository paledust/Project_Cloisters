using UnityEngine;

public class Dragger
{
    public Joint joint;
    public Rigidbody rigidbody;
}
public class PhysicDragManager : Singleton<PhysicDragManager>
{
    private Dragger dragger;
    public void SyncDraggerPos(Vector3 position)
    {
        if (dragger == null)
            dragger = GetNewDragger();
        dragger.rigidbody.transform.position = position;
        dragger.rigidbody.position = position;
    }

    public Dragger ConnectToRigid(Rigidbody m_rigid, Vector3 connectAnchor)
    {
        if (dragger == null)
            dragger = GetNewDragger();
        dragger.joint.connectedBody = m_rigid;
        dragger.joint.connectedAnchor = connectAnchor;

        return dragger;
    }
    public static Dragger PinRigidToCurrentPos(Rigidbody m_rigid, float springStrength = 1000, float damper = 100)
    {
        var dragger = GetNewSpringDragger(springStrength, damper);

        dragger.rigidbody.position = m_rigid.position;
        dragger.rigidbody.transform.position = m_rigid.position;
        dragger.joint.connectedBody = m_rigid;
        dragger.joint.connectedAnchor = Vector3.zero;

        return dragger;
    }
    public void BreakJoint()
    {
        if (dragger != null)
        {
            dragger.joint.connectedBody = null;
        }
    }
    public void CleanUp()
    {
        if(dragger!=null)
        {
            Destroy(dragger.rigidbody.gameObject);
            dragger = null;
        }
    }
    static Dragger GetNewDragger()
    {
        var draggerRigid = new GameObject("Dragger").AddComponent<Rigidbody>();
        draggerRigid.isKinematic = true;
        var joint = draggerRigid.gameObject.AddComponent<HingeJoint>();
        joint.axis = Vector3.forward;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;

        var _dragger = new Dragger()
        {
            joint = joint,
            rigidbody = draggerRigid
        };
        return _dragger;
    }
    static Dragger GetNewSpringDragger(float springStrength, float damper)
    {
        var draggerRigid = new GameObject("Dragger").AddComponent<Rigidbody>();
        draggerRigid.isKinematic = true;
        var joint = draggerRigid.gameObject.AddComponent<SpringJoint>();
        joint.axis = Vector3.forward;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.spring = springStrength;
        joint.damper = damper;

        var _dragger = new Dragger()
        {
            joint = joint,
            rigidbody = draggerRigid
        };
        return _dragger;
    }
    public static Joint GetNewBodyConnector(Rigidbody fromBody, Rigidbody toBody, float springStrength, float damper, float minimumDistance)
    {
        var joint = fromBody.gameObject.AddComponent<ConfigurableJoint>();
        joint.axis = Vector3.forward;
        joint.autoConfigureConnectedAnchor = false;
        joint.anchor = Vector3.zero;
        joint.connectedBody = toBody;
        joint.connectedAnchor = Vector3.zero;
        joint.axis = Vector3.forward;
        joint.secondaryAxis = Vector3.up;
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        joint.linearLimit = new SoftJointLimit()
        {
            limit = Mathf.Max((fromBody.position - toBody.position).magnitude, minimumDistance),
        };
        joint.linearLimitSpring = new SoftJointLimitSpring()
        {
            spring = springStrength,
            damper = damper
        };
        joint.enableCollision = true;

        return joint;
    }
}