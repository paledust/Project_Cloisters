using UnityEngine;

public class BounceBall : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float maxSpeed = 100f;
    [SerializeField] private float minSpeed = 10f;

    [Header("Shrink")]
    [SerializeField] private float shrinkFactor = 0.2f;
    [SerializeField] private Vector2 shrinkVelRange;
    [SerializeField] private Transform renderTrans;

    public float vel => m_rigid.velocity.magnitude;

    private BuffProperty currentSpeed;
    private Rigidbody m_rigid;

    void Awake()
    {
        currentSpeed = new BuffProperty(0, maxSpeed);
        m_rigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        m_rigid.velocity = Vector2.Lerp(m_rigid.velocity, m_rigid.velocity.normalized * currentSpeed.cachedValue, Time.fixedDeltaTime * 1);
    }
    void LateUpdate()
    {
        Vector2 vel = m_rigid.velocity;
        float angle = Mathf.Atan2(vel.y, vel.x);
        renderTrans.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        float shape = Mathf.Lerp(0, 1, Mathf.InverseLerp(shrinkVelRange.x, shrinkVelRange.y, m_rigid.velocity.magnitude)) * shrinkFactor;
        renderTrans.localScale = new Vector3(1 + shape, 1 - shape, 1);
    }

    #region Physics Handler
    public void Bounce(Vector2 impulse, float speedBoost, AttributeModifyType modifyType = AttributeModifyType.Add)
    {
        currentSpeed.ModifiValue(speedBoost, modifyType);
        m_rigid.velocity = impulse.normalized * currentSpeed.cachedValue * 2;
    }
    #endregion
}