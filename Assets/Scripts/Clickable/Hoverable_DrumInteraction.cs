using DG.Tweening;
using UnityEngine;

public class Hoverable_DrumInteraction : MonoBehaviour
{
    [SerializeField] private string hoverSFX;

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

    private Basic_Clickable self;
    private Vector3 originalScale;
    private int beatCounter = 0;
    // Start is called before the first frame update
    void Start()
    {
        self = GetComponent<Basic_Clickable>();
        originalScale = transform.localScale;
        self.onHover += PlayHoverSFX;
        EventHandler.E_OnDrumKnocked += HarmDrum;
        EventHandler.E_OnDrumBeat += StaticDrum;
    }
    void OnDestroy()
    {
        self.onHover -= PlayHoverSFX;
        EventHandler.E_OnDrumKnocked -= HarmDrum;
        EventHandler.E_OnDrumBeat -= StaticDrum;
    }
    void HarmDrum(float strength)
    {
        ShakeDrum(strength * harmScale, harmVibration, hoverScaleDuration);
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
    void PlayHoverSFX(PlayerController player)
    {
        StylizedDrumController.Instance.QueueBeat(hoverSFX, Mathf.Clamp(player.PointerDelta.magnitude * speedToVolume, volumeRange.x, volumeRange.y));
        ShakeDrum(player.PointerDelta.magnitude * speedToVolume, 90, hoverScaleDuration);
    }
}
