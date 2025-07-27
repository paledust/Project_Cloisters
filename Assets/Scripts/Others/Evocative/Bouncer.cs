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
    private Rigidbody m_rigid;

    private Action<BounceBall> onBounce;

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
    }
    void OnDestroy()
    {
        spriteRenderer.transform.DOKill();
        blinkRender.DOKill();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (!colliding)
        {
            colliding = true;
            Vector3 normal = collision.GetContact(0).normal;
            Vector2 vel = m_rigid.velocity + collision.relativeVelocity;
            vel = Vector2.Reflect(vel, normal).normalized;

            float angle = Vector2.SignedAngle(normal, vel);
            angle = Mathf.Sign(angle) * Mathf.Clamp(Mathf.Abs(angle), reflectAngle.x, reflectAngle.y);
            vel = Quaternion.Euler(0, 0, angle) * normal;

            var bounceBall = collision.gameObject.GetComponent<BounceBall>();
            if (bounceBall != null)
            {
                var rootTrans = spriteRenderer.transform;
                var blinker = blinkRender;
                bounceBall.Bounce(vel, bounceSpeedBonus, bounceSpeedBoost);
                blinker.DOKill();
                blinker.DOFade(1, 0.1f).OnComplete(() => blinker.DOFade(0, 0.05f));

                rootTrans.localScale = Vector3.one;
                rootTrans.DOKill();
                rootTrans.DOPunchScale(bounceSize * Vector3.one, 0.1f, 1, 2).SetEase(Ease.OutQuad);
                
                onBounce?.Invoke(bounceBall);
            }
        }
    }
    public void SwapRender(SpriteRenderer rootRender, SpriteRenderer blinkRender)
    {
        this.spriteRenderer = rootRender;
        this.blinkRender = blinkRender;
    }
    public void OnBounce(Action<BounceBall> action) => onBounce = action;
    void OnCollisionExit(Collision collision)
    {
        if (colliding)
            colliding = false;
    }
}
