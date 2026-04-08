using DG.Tweening;
using UnityEngine;

public class Clickable_ExperimentalShapeDragger : Basic_Clickable
{
    [SerializeField] private Rigidbody m_rigid;
    [SerializeField] private Transform dragCenter;
    [SerializeField] private float maxDragOffset;

    private ShapeInteractionHandler experimentalShape;
    private Dragger dragger;
    private Vector3 offset;
    private const float DEPTH = 32;

    void Start()
    {
        experimentalShape = m_rigid.GetComponent<ShapeInteractionHandler>();
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        player.HoldInteractable(this);
        
        Vector3 hitPoint = dragCenter.position + Vector3.ClampMagnitude(hitPos - dragCenter.position, maxDragOffset);

        PhysicDragManager.Instance.SyncDraggerPos(hitPoint);
        dragger = PhysicDragManager.Instance.ConnectToRigid(m_rigid, m_rigid.transform.InverseTransformPoint(hitPoint));
        offset = dragger.rigidbody.position - player.GetCursorWorldPoint(DEPTH);
        offset.z = 0;
        
        m_rigid.drag = 10;
        m_rigid.angularDrag = 10;

        experimentalShape.OnControlled();
    }
    public override void OnHover(PlayerController player)
    {
        base.OnHover(player);
    }
    public override void OnExitHover()
    {
        base.OnExitHover();
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        experimentalShape.OnRelease();
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
            Vector3 newPos = Vector3.Lerp(dragger.rigidbody.position, player.GetCursorWorldPoint(DEPTH) + offset, Time.deltaTime * 10);
            dragger.rigidbody.MovePosition(newPos);
        }
    }
}
