using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Clickable_Crystal : Basic_Clickable
{
    [SerializeField] private float depth;
    [SerializeField] private float lerpSpeed = 10;

    [Header("Crystal")]
    [SerializeField] private Transform crystalTrans;
    [SerializeField] private float selfRotateSpeed = 5;

    [Header("Particle")]
    [SerializeField] private ParticleSystem blinkParticle;
    [SerializeField] private float particleExpandForce = 10f;
    [SerializeField] private float particleAngleScale = 0.25f;
    [SerializeField] private float particleFollowSpeed = 2;

    [Header("Interaction")]
    [SerializeField] private float xPosToRotation = 4;

    [Header("Spoter")]
    [SerializeField] private SpriteRenderer spotterRender;

    private Vector3 targetPos;
    private float particleAngle;
    private float angle;

    void Awake()
    {
        spotterRender.transform.localScale = Vector3.zero;
        spotterRender.color = new Color(1, 1, 1, 0);
    }
    void Start()
    {
        targetPos = transform.position;
        angle = Random.Range(-50f, 50f);
        particleAngle = (angle + (transform.position.x * xPosToRotation)) * particleAngleScale;
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        transform.DOKill();
        transform.DOScale(1.2f, 0.25f).SetEase(Ease.OutBack);
        spotterRender.DOKill();
        spotterRender.transform.DOScale(1.7f, 0.25f).SetEase(Ease.OutBack);
        spotterRender.DOFade(1, 0.25f);
        
        var particles = new ParticleSystem.Particle[blinkParticle.main.maxParticles];
        int count = blinkParticle.GetParticles(particles);
        for(int i = 0; i < count; i++)
        {
            particles[i].velocity = particles[i].position * particleExpandForce;
        }
        blinkParticle.SetParticles(particles, count);

        player.HoldInteractable(this);
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        transform.DOKill();
        transform.DOScale(1f, 0.25f).SetEase(Ease.OutBack);
        spotterRender.DOKill();
        spotterRender.transform.DOScale(0, 0.5f).SetEase(Ease.OutBack);
        spotterRender.DOFade(0, 0.25f);
    }
    public override void ControllingUpdate(PlayerController player)
    {
        targetPos = player.GetCursorWorldPoint(depth);
    }
    void Update()
    {
        transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * lerpSpeed);
        angle += selfRotateSpeed * Time.deltaTime;
        float targetAngle = angle + (transform.position.x * xPosToRotation);
        crystalTrans.localRotation = Quaternion.Euler(0, targetAngle, 0);

        particleAngle = Mathf.Lerp(particleAngle, targetAngle * particleAngleScale, Time.deltaTime * particleFollowSpeed);
        blinkParticle.transform.localRotation = Quaternion.Euler(0, particleAngle, 0);
    }
}