using UnityEngine;

public class Clickable_Moveable : Basic_Clickable
{
    [SerializeField] private Rigidbody m_rigid;
    [SerializeField] private float massCenterRadius;
    private Dragger dragger;
    private Vector3 pos;

    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        player.HoldInteractable(this);
        Vector3 localHit = transform.InverseTransformPoint(hitPos);
        if((localHit - m_rigid.centerOfMass).sqrMagnitude < massCenterRadius * massCenterRadius)
            localHit = m_rigid.centerOfMass + (localHit - m_rigid.centerOfMass).normalized*0.05f;

        dragger = PhysicDragManager.Instance.ConnectToRigid(m_rigid, localHit);
        dragger.rigidbody.transform.position = transform.TransformPoint(localHit);
        m_rigid.drag = 10;
        m_rigid.angularDrag = 10;
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        PhysicDragManager.Instance.BreakJoint();
        m_rigid.drag = 5;
        m_rigid.angularDrag = 7;
        dragger = null;
    }
    public override void ControllingUpdate(PlayerController player)
    {
        base.ControllingUpdate(player);
        if(dragger != null)
        {
            Vector3 newPos = Vector3.Lerp(dragger.rigidbody.position, player.GetCursorWorldPoint(32), Time.deltaTime * 10);
            dragger.rigidbody.MovePosition(newPos);
        }
    }
    void OnDrawGizmos()
    {
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(m_rigid.centerOfMass, massCenterRadius);
        Gizmos.DrawWireSphere(pos, 0.2f);
    }
}