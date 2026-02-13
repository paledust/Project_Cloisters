using System;
using DG.Tweening;
using UnityEngine;

using Random = UnityEngine.Random;

public class Clickable_Stylized : Basic_Clickable
{
    [SerializeField] private bool doubleTap = true;
    [SerializeField] private string hoverSFX;

    [Header("Scale Animation")]
    [SerializeField] private float hoverScalePunch = 1.1f;
    [SerializeField] private float hoverScaleDuration = 0.25f;
    [SerializeField] private bool initScaleOnAwake = false;

    [Header("Volume Settings")]
    [SerializeField] private Vector2 volumeRange = new Vector2(0, 1);
    [SerializeField] private float speedToVolume = 1f; 

    private PlayBeatCommand playBeatCommand;
    private Vector3 originalScale;
    private bool secondBeat = false;
    private bool isHovering = false;
    private float beatTimer = 0f;
    
    public Action OnDrumBeat;
    public Action OnDrumEnable;
    
    private const double BEAT_TEMPLE = 60f/420f;

    void Awake()
    {
        if(initScaleOnAwake)
            originalScale = transform.localScale;
    }
    void OnEnable()
    {
        if(!initScaleOnAwake)
            originalScale = transform.localScale;
        OnDrumEnable?.Invoke();
    }
    void Update()
    {
        if(!doubleTap) return;
        if(isHovering && !secondBeat)
        {
            beatTimer += Time.deltaTime;
            if(beatTimer >= BEAT_TEMPLE)
            {
                StylizedDrumController.Instance.QueueBeat(sfx_clickSound, 0.5f, playBeatCommand);
                ShakeDrum(1);
                secondBeat = true;
            }
        }
    }
    public override void OnHover(PlayerController player)
    {
        base.OnHover(player);
        
        float strength = player.PointerDelta.magnitude * speedToVolume;
        playBeatCommand = StylizedDrumController.Instance.QueueBeat(hoverSFX, Mathf.Clamp(strength, volumeRange.x, volumeRange.y));
        EventHandler.Call_OnDrumKnocked(strength);

        ShakeDrum(strength);
        isHovering = true;
        secondBeat = false;
        beatTimer = 0f;
    }
    public override void OnExitHover()
    {
        base.OnExitHover();
        isHovering = false;
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        ShakeDrum(1);
    }
    public void PopGeo()
    {
        transform.DOScale(originalScale, Random.Range(0.15f, 0.25f)).SetEase(Ease.OutBack).OnComplete(() =>
        {
            this.enabled = true;
            EnableHitbox();
        });
    }
    void ShakeDrum(float strength)
    {
        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOPunchScale(Vector3.one * hoverScalePunch * Mathf.Clamp01(strength), hoverScaleDuration, 2);
        transform.DOShakeRotation(hoverScaleDuration, Random.Range(10f,20f), 90, 90, true, ShakeRandomnessMode.Harmonic);
        OnDrumBeat?.Invoke();
    }
}
