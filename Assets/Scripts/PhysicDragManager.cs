using UnityEngine;

public class Dragger
{
    public Joint joint;
    public Rigidbody rigidbody;
}
public class PhysicDragManager : Singleton<PhysicDragManager>
{
    private Dragger dragger;
    public Dragger ConnectToRigid(Rigidbody m_rigid, Vector3 connectAnchor)
    {
        if(dragger == null)
        {
            var draggerRigid = new GameObject("Dragger").AddComponent<Rigidbody>();
            draggerRigid.isKinematic = true;
            var joint = draggerRigid.gameObject.AddComponent<HingeJoint>();
            joint.axis = Vector3.forward;
            joint.autoConfigureConnectedAnchor = false;
            joint.anchor = Vector3.zero;

            dragger = new Dragger()
            {
                joint = joint,
                rigidbody = draggerRigid
            };
        }
        dragger.joint.connectedBody = m_rigid;
        dragger.joint.connectedAnchor = connectAnchor;

        return dragger;
    }
    public void BreakJoint()
    {
        if(dragger != null)
        {
            dragger.joint.connectedBody = null;
        }
    }
}