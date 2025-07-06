using UnityEngine;

public class Clickable_BounceBall : Basic_Clickable
{
    [SerializeField] private float minimumDragDistance = 0.1f; // Minimum distance to consider a drag
    [SerializeField] private float forceMultiplier = 1f;
    [SerializeField] private float maxForce = 10f;
    private Rigidbody m_rigid;
    private Vector2 dragPoint;
    private Vector2 dragStartPoint;
    private float camDepth;
    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
    }
    void Start()
    {
        camDepth = Camera.main.WorldToScreenPoint(transform.position).z;
    }
    #region Interaction
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        player.HoldInteractable(this);
        dragStartPoint = hitPos;
    }
    public override void ControllingUpdate(PlayerController player)
    {
        dragPoint = player.GetCursorWorldPoint(camDepth);
    }
    public override void OnRelease(PlayerController player)
    {
        dragPoint = player.GetCursorWorldPoint(camDepth);
        Vector2 diff = dragStartPoint - dragPoint;
        diff = Vector2.ClampMagnitude(diff, maxForce);

        if (diff.magnitude > minimumDragDistance)
        {
            m_rigid.AddForce(diff * forceMultiplier, ForceMode.Impulse);
        }
    }
    #endregion

    #region Physics Handler
    public void HandleBounce(Vector2 impulse, float speedBoost)
    {
        m_rigid.velocity = impulse.normalized * speedBoost;
    }
    #endregion
}