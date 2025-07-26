using UnityEngine;

public class BounceBall : MonoBehaviour
{
    [Header("Speed")]
    [SerializeField] private float maxSpeed = 100f;

    [Header("Boost")]
    [SerializeField] private float boostMulti = 4f;
    [SerializeField] private float boostDeacc = 1;

    [Header("Shrink")]
    [SerializeField] private float shrinkFactor = 0.2f;
    [SerializeField] private Vector2 shrinkVelRange;
    [SerializeField] private Transform renderTrans;

    public float vel => m_rigid.velocity.magnitude;

    private BuffProperty currentSpeed;
    private Rigidbody m_rigid;
    private float realSpeed;

    void Awake()
    {
        currentSpeed = new BuffProperty(10, maxSpeed);
        m_rigid = GetComponent<Rigidbody>();
    }
    void FixedUpdate()
    {
        if (realSpeed > currentSpeed.cachedValue)
        {
            realSpeed -= Time.fixedDeltaTime * boostDeacc;
            if (realSpeed < currentSpeed.cachedValue)
            {
                realSpeed = currentSpeed.cachedValue;
            }
        }
        m_rigid.velocity = m_rigid.velocity.normalized * realSpeed;
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
    public void Bounce(Vector2 impulse, float speedMod, float speedBoost = 1, AttributeModifyType modifyType = AttributeModifyType.Add)
    {
        currentSpeed.ModifiValue(speedMod, modifyType);
        realSpeed = Mathf.Max(realSpeed, currentSpeed.cachedValue * boostMulti * speedBoost);
        m_rigid.velocity = impulse.normalized * realSpeed;
    }
    #endregion
    public void ResetAtPos(Vector3 position)
    {
        transform.position = position;
        m_rigid.position = position;
        currentSpeed.ResetValue();
        realSpeed = currentSpeed.cachedValue;
        m_rigid.velocity = Vector3.zero;
    }
}