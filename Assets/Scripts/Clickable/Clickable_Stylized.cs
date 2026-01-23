using SimpleAudioSystem;
using DG.Tweening;
using UnityEngine;

public class Clickable_Stylized : Basic_Clickable
{
    [SerializeField] private string hoverSFX;
    [Header("Scale Animation")]
    [SerializeField] private float hoverScalePunch = 1.1f;
    [SerializeField] private float hoverScaleDuration = 0.25f;
    [Header("Volume Settings")]
    [SerializeField] private Vector2 volumeRange = new Vector2(0, 1);
    [SerializeField] private float speedToVolume = 1f; 

    private Vector3 originalScale;

    void OnEnable()
    {
        originalScale = transform.localScale;    
    }
    public override void OnHover(PlayerController player)
    {
        base.OnHover(player);
        
        float strength = player.PointerDelta.magnitude * speedToVolume;
        StylizedDrumController.Instance.QueueBeat(hoverSFX, Mathf.Clamp(strength, volumeRange.x, volumeRange.y));
        EventHandler.Call_OnDrumKnocked(strength);

        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOPunchScale(Vector3.one * hoverScalePunch * Mathf.Clamp01(strength), hoverScaleDuration, 2);
        transform.DOShakeRotation(hoverScaleDuration * Mathf.Clamp01(strength), Random.Range(10f,20f), 90, 90, true, ShakeRandomnessMode.Harmonic);
    }
}
