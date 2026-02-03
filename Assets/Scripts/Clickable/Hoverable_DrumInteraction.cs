using DG.Tweening;
using UnityEngine;

public class Hoverable_DrumInteraction : MonoBehaviour
{
    public enum DrumState
    {
        Beating,
        MaxBeating,
        MaxCharged,
    }
    [SerializeField] private string hoverSFX;
    [SerializeField] private string breakSFX;

    [Header("Scale Animation")]
    [SerializeField] private float hoverScalePunch = 1.1f;
    [SerializeField] private float hoverScaleDuration = 0.25f;

    [Header("Static Temple Scale")]
    [SerializeField] private int beatGap = 4;
    [SerializeField] private float staticScalePunch = 0.05f;
    [SerializeField] private float staticScaleDuration = 0.15f;
    [SerializeField] private int staticScaleVibration = 5;

    [Header("Volume Settings")]
    [SerializeField] private Vector2 volumeRange = new Vector2(0, 1);
    [SerializeField] private float speedToVolume = 1f; 

    [Header("Harm Settings")]
    [SerializeField] private float harmScale = 0.5f;
    [SerializeField] private int harmVibration = 10;

    [Header("Drum Charge")]
    [SerializeField] private DrumState drumState;
    [SerializeField] private float beatPowerAdd = 0;
    [SerializeField] private float beatDrainSpeed = 1;
    [SerializeField] private float beatMaxThreasholdTime = 2;
    [SerializeField] private float beatMaxLastTime = 2f;

    [Header("Drum Charge View")]
    [SerializeField] private SpriteRenderer glowSprite;
    [SerializeField] private ParticleSystem rippleParticle;
    [SerializeField] private PerRendererColor heroColor;
    [SerializeField] private Transform heroSpehreRenderTrans;
    [SerializeField] private Color chargeHeroColor;
    [SerializeField] private Color normalHeroColor;
    [SerializeField] private float glowMaxAlpha = 0.5f;
    private Color chargeColor;

    private Basic_Clickable self;
    private Vector3 originalScale;
    private float accumulatePower = 0;
    private float chargeTimer = 0;
    private int beatCounter = 0;

    public float m_accumulatePower => accumulatePower;

    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Basic_Clickable>();
        originalScale = transform.localScale;
        self.onHover += KnockBassDrum;
        EventHandler.E_OnDrumKnocked += HarmDrum;
        EventHandler.E_OnDrumBeat += StaticDrum;
        chargeColor = glowSprite.color;
        chargeColor.a = glowMaxAlpha;
    }
    void Update()
    {
        //Charge Drum
        if(accumulatePower>0)
        {
            accumulatePower -= beatDrainSpeed * Time.deltaTime;
            accumulatePower = Mathf.Clamp(accumulatePower, 0, 1.5f);
        }
        Color targetColor = glowSprite.color;
        targetColor.a = Mathf.Clamp01(accumulatePower) * glowMaxAlpha;
        glowSprite.color = Color.Lerp(glowSprite.color, targetColor, Time.deltaTime * 20);
        switch(drumState)
        {
            case DrumState.Beating:
                if(accumulatePower >= 1)
                {
                    drumState = DrumState.MaxBeating;
                    chargeTimer = 0;
                }
                break;
            case DrumState.MaxBeating:
                if(accumulatePower >= 1)
                {
                    chargeTimer += Time.deltaTime;
                    if(chargeTimer >= beatMaxThreasholdTime)
                    {
                        drumState = DrumState.MaxCharged;
                        chargeTimer = 0;
                        DOTween.Kill(heroColor);
                        DOTween.To(()=> heroColor.tint, x=> heroColor.tint = x, chargeHeroColor, 0.3f).SetId(heroColor);
                        heroSpehreRenderTrans.DOKill();
                        heroSpehreRenderTrans.DOScale(Vector3.one*1.2f, 0.3f).SetEase(Ease.OutBack);
                    }
                }
                else
                {
                    drumState = DrumState.Beating;
                    chargeTimer = 0;
                }
                break;
            case DrumState.MaxCharged:
                if(accumulatePower < 1)
                {
                    chargeTimer += Time.deltaTime;
                    if(chargeTimer >= beatMaxLastTime)
                    {
                        drumState = DrumState.Beating;
                        chargeTimer = 0;
                        DOTween.Kill(heroColor);
                        DOTween.To(()=> heroColor.tint, x=> heroColor.tint = x, normalHeroColor, 1f).SetId(heroColor);
                        heroSpehreRenderTrans.DOKill();
                        heroSpehreRenderTrans.DOScale(Vector3.one, 1f).SetEase(Ease.OutQuad);
                    }
                }
                else
                {
                    chargeTimer = 0;
                }
                break;
        }
    }
    void OnDestroy()
    {
        self.onHover -= KnockBassDrum;
        EventHandler.E_OnDrumKnocked -= HarmDrum;
        EventHandler.E_OnDrumBeat -= StaticDrum;
    }
    void HarmDrum(float strength)
    {
        ShakeDrum(strength * harmScale, harmVibration, hoverScaleDuration);
        accumulatePower += beatPowerAdd * strength;
        accumulatePower = Mathf.Min(1.5f, accumulatePower);
        if(drumState == DrumState.MaxCharged)
        {
            rippleParticle.Emit(1);
        }
    }
    void StaticDrum()
    {
        beatCounter++;
        if(beatCounter % beatGap != 0) return;
        ShakeDrum(staticScalePunch, staticScaleVibration, staticScaleDuration);
    }
    void ShakeDrum(float strength, int vibration, float duration)
    {
        strength = Mathf.Clamp01(strength);
        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOPunchScale(Vector3.one * hoverScalePunch * strength, duration, vibration, 4);
    }
    void KnockBassDrum(PlayerController player)
    {
        StylizedDrumController.Instance.QueueBeat(hoverSFX, Mathf.Clamp(player.PointerDelta.magnitude * speedToVolume, volumeRange.x, volumeRange.y));
        if(drumState == DrumState.MaxCharged)
        {
            EventHandler.Call_OnBassChargeBeat();
            accumulatePower = 0;
            ShakeDrum(player.PointerDelta.magnitude * speedToVolume, 90, hoverScaleDuration);
        }
        else
        {
            ShakeDrum(player.PointerDelta.magnitude * speedToVolume, 90, hoverScaleDuration);
        }
    }
}
