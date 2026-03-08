using UnityEngine;
using DG.Tweening;

public class Clickable_Crystal : Basic_Clickable
{
    [SerializeField] private float depth;
    [SerializeField] private float lerpSpeed = 10;

    [Header("Particle")]
    [SerializeField] private ParticleSystem blinkParticle;
    [SerializeField] private float particleExpandForce = 10f;

    [Header("Spoter")]
    [SerializeField] private SpriteRenderer spotterRender;
    [SerializeField] private WhimsicalTextSpoter whimsicalTextSpoter;

    private Vector3 targetPos;

    void Awake()
    {
        spotterRender.transform.localScale = Vector3.zero;
        spotterRender.color = new Color(1, 1, 1, 0);
    }
    void Start()
    {
        whimsicalTextSpoter.enabled = false;
    }
    void OnEnable()
    {
        targetPos = transform.position;
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        player.HoldInteractable(this);
        whimsicalTextSpoter.enabled = true;

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
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        whimsicalTextSpoter.enabled = false;

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
    }
}