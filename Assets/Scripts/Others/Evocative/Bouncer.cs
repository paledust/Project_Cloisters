using UnityEngine;
using DG.Tweening;
using System;
using System.Collections.Generic;
using SimpleAudioSystem;

public class Bouncer : MonoBehaviour
{
    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer blinkRender;
    [SerializeField] private float bounceSize = 1;

    [Header("Bounce Settings")]
    [SerializeField] private float bounceSpeedBoost = 2;
    [SerializeField] private float bounceSpeedBonus = 0;
    [SerializeField, Range(0, 1)] private float steerControl = 0.5f;
	
	[Header("VFX")]
    [SerializeField] private ParticleSystem vfxBounce;
    
    [Header("Audio")]
    [SerializeField] private AudioData_SO sfxBounceData;

    private bool colliding = false;
    [SerializeField, ShowOnly] private bool canBounce = true;
    private Vector3 initRootSize;
    private Rigidbody m_rigid;

    public event Action<BounceBall> onBounce;
    public event Action<BounceBall> onPreBounce;

    void Awake()
    {
        m_rigid = GetComponent<Rigidbody>();
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
    public void DisableCollision()
    {
        SwitchCanBounce(false);
        m_rigid.detectCollisions = false;
    }
    public void PlayBounceFeedback()
    {
        if(vfxBounce!=null)
            vfxBounce.Play();
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
                vel = ((Vector2)m_rigid.velocity.normalized * steerControl + vel.normalized).normalized * vel.magnitude;

                PlayBounceFeedback();
                bounceBall.Bounce(vel, bounceSpeedBonus, bounceSpeedBoost);
                if(sfxBounceData!=null)
                    AudioManager.Instance.PlaySFX(sfxBounceData.AudioKey, Mathf.Clamp(collision.relativeVelocity.magnitude * 0.1f, 0.01f, 1f));
                onBounce?.Invoke(bounceBall);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        var bounceBall = collision.gameObject.GetComponent<BounceBall>();
        if (colliding && bounceBall != null)
            colliding = false;
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
}
