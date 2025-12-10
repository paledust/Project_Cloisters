using UnityEngine;
using DG.Tweening;
using System;

public class Bouncer : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer blinkRender;
    [SerializeField] private float bounceSize = 1;

    [Header("Bounce Settings")]
    [SerializeField] private Vector2 reflectAngle = new Vector2(90, 180);
    [SerializeField] private float bounceSpeedBoost = 2;
    [SerializeField] private float bounceSpeedBonus = 0;

    private bool colliding = false;
    [SerializeField, ShowOnly] private bool canBounce = true;
    private Vector3 initRootSize;
    private Rigidbody m_rigid;
    private Collider m_collider;

    public bool m_colliding => colliding;
    public event Action<BounceBall> onBounce;
    public event Action<BounceBall> onPreBounce;

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
        m_collider = GetComponent<Collider>();
        initRootSize = spriteRenderer.transform.localScale;
    }
    void OnDestroy()
    {
        onBounce = null;
        spriteRenderer.transform.DOKill();
        blinkRender.DOKill();
    }
    public void SwitchCanBounce(bool isBounce)
    {
        canBounce = isBounce;
    }
    public void SwitchOffCollider()
    {
        SwitchCanBounce(false);
        m_collider.enabled = false;
    }
    public void PlayBounceFeedback()
    {
        var rootTrans = spriteRenderer.transform;
        var blinker = blinkRender;
        blinker.DOKill();
        blinker.DOFade(1, 0.1f).OnComplete(() => blinker.DOFade(0, 0.05f));
        rootTrans.localScale = initRootSize;
        rootTrans.DOKill();
        rootTrans.DOPunchScale(bounceSize * initRootSize, 0.1f, 1, 2).SetEase(Ease.OutQuad);
    }
    void OnCollisionEnter(Collision collision)
    {
        var bounceBall = collision.gameObject.GetComponent<BounceBall>();
        if (!colliding && bounceBall != null)
        {
            colliding = true;
            onPreBounce?.Invoke(bounceBall);

            if(canBounce)
            {
                Vector3 normal = collision.GetContact(0).normal;
                Vector2 vel = m_rigid.velocity + collision.relativeVelocity;
                vel = Vector2.Reflect(vel, normal).normalized;

                float angle = Vector2.SignedAngle(normal, vel);
                angle = Mathf.Sign(angle) * Mathf.Clamp(Mathf.Abs(angle), reflectAngle.x, reflectAngle.y);
                vel = Quaternion.Euler(0, 0, angle) * normal;

                PlayBounceFeedback();
                bounceBall.Bounce(vel, bounceSpeedBonus, bounceSpeedBoost);
                onBounce?.Invoke(bounceBall);
            }
        }
    }
    public void SwapRender(SpriteRenderer rootRender, SpriteRenderer blinkRender)
    {
        this.spriteRenderer = rootRender;
        this.blinkRender = blinkRender;
    }
    public void ChangeBounceParam(float newSpeedBonus, float newSpeedBoost)
    {
        bounceSpeedBonus = newSpeedBonus;
        bounceSpeedBoost = newSpeedBoost;
    }
    void OnCollisionExit(Collision collision)
    {
        if (colliding)
            colliding = false;
    }
}
