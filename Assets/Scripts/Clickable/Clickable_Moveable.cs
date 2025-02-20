using UnityEngine;

public class Clickable_Moveable : Basic_Clickable
{
    [SerializeField] private Rigidbody m_rigid;
    private Dragger dragger;

    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        player.HoldInteractable(this);
        dragger = PhysicDragManager.Instance.ConnectToRigid(m_rigid, transform.InverseTransformPoint(hitPos));
        dragger.rigidbody.transform.position = hitPos;
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        PhysicDragManager.Instance.BreakJoint();
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
}