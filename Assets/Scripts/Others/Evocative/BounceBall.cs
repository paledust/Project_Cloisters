using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Rendering;

public class BounceBall : MonoBehaviour
{
    [Header("Lock")]
    [SerializeField] private ParentConstraint constraint;

    [Header("Speed")]
    [SerializeField] private float maxSpeed = 100f;

    [Header("Boost")]
    [SerializeField] private float boostMulti = 4f;
    [SerializeField] private float boostDeacc = 1;

    [Header("Shrink")]
    [SerializeField] private float shrinkFactor = 0.2f;
    [SerializeField] private Vector2 shrinkVelRange;
    [SerializeField] private SpriteRenderer ballRender;
    [SerializeField] private SpriteRenderer ballGlow;
    [SerializeField] private SortingGroup sortingGroup;

    [Header("Final")]
    [SerializeField] private ParticleSystem p_final;

    [Header("Respawn")]
    [SerializeField] private Animation m_respawnAnime;
    [SerializeField] private ParticleSystem p_respawn;

    [Header("Charge")]
    [SerializeField] private ParticleSystem p_trail;

    public float consistentSpeed => currentSpeed.cachedValue;
    public bool m_isSuperCharge => isSuperCharge;

    private BuffProperty currentSpeed;
    private Rigidbody m_rigid;
    private float realSpeed;
    private bool isSuperCharge;

    private const string ANIME_RESPAWN = "EVO_BallRespawn";
    private const string ANIME_GLOW = "EVO_BallGlow";


    void Awake()
    {
        currentSpeed = new BuffProperty(0, maxSpeed);
        m_respawnAnime[ANIME_RESPAWN].layer = 0;
        m_respawnAnime[ANIME_GLOW].layer = 1;
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
        ballRender.transform.rotation = Quaternion.Euler(0, 0, angle * Mathf.Rad2Deg);
        float shape = Mathf.Lerp(0, 1, Mathf.InverseLerp(shrinkVelRange.x, shrinkVelRange.y, m_rigid.velocity.magnitude)) * shrinkFactor;
        ballRender.transform.localScale = new Vector3(1 + shape, 1 - shape, 1);
    }

    #region Physics Handler
    public void Bounce(Vector2 impulse, float speedMod, float speedBoost = 1, AttributeModifyType modifyType = AttributeModifyType.Add)
    {
        currentSpeed.ModifiValue(speedMod, modifyType);
        realSpeed = Mathf.Max(realSpeed, currentSpeed.cachedValue * boostMulti * speedBoost);
        m_rigid.velocity = impulse.normalized * realSpeed;
    }
    #endregion

    public void BallFinal()
    {
        this.enabled = false;
        ballRender.transform.localScale = Vector3.one;
        ballRender.transform.DOScale(2, 1.5f).SetEase(Ease.OutQuad);
        sortingGroup.sortingLayerName = "VFX";
        sortingGroup.sortingOrder = 20;
        p_final.Play();
        m_rigid.drag = 3;
    }
    public void ResetAtPos(Vector3 position)
    {
        transform.position = position;
        m_rigid.position = position;
        currentSpeed.ResetValue();
        realSpeed = currentSpeed.cachedValue;
        m_rigid.velocity = Vector3.zero;
        constraint.constraintActive = true;

        //Ball Spawn Effect
        p_respawn.Play();
        m_respawnAnime.Play(ANIME_RESPAWN);
    }
    public void Launch(Vector2 force)
    {
        constraint.constraintActive = false;
        Bounce(force, force.magnitude, 2, AttributeModifyType.Add);
    }
    public void ChargeBounceBall()
    {
        if(isSuperCharge) return;
        isSuperCharge = true;
        p_trail.gameObject.SetActive(true);
        m_respawnAnime.Play(ANIME_GLOW);
    }
}