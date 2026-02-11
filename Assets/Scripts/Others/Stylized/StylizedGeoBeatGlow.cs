using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StylizedGeoBeatGlow : MonoBehaviour
{
    [SerializeField] private Clickable_Stylized clickableStylized;
    [SerializeField] private PerRendererStylizedShade stylizedShade;
    [SerializeField, ColorUsage(false, true)] private Color beatGlowColor;
    [SerializeField] private float dim = 0.5f;
    [SerializeField] private float duration = 0.1f;
    private Color originalColor;
    void Awake()
    {
        clickableStylized.OnDrumBeat += BeatGlow;
        clickableStylized.OnDrumEnable += DrumEnable;
        originalColor = stylizedShade.Tint;
        stylizedShade.Tint = originalColor * dim;
    }
    void OnDestroy()
    {
        clickableStylized.OnDrumBeat -= BeatGlow;
        clickableStylized.OnDrumEnable -= DrumEnable;
    }
    void DrumEnable()
    {
        stylizedShade.DOKill();
        DOTween.To(()=> stylizedShade.Tint, x=> stylizedShade.Tint = x, originalColor, 0.15f).OnComplete(()=>
        {
            stylizedShade.Tint = originalColor;
        }).SetId(stylizedShade);
    }
    void BeatGlow()
    {
        stylizedShade.DOKill();
        stylizedShade.Tint = originalColor;
        DOTween.To(()=> stylizedShade.Tint, x=> stylizedShade.Tint = x, beatGlowColor, duration).SetLoops(2, LoopType.Yoyo).SetEase(Ease.OutQuad).OnComplete(()=>
        {
            stylizedShade.Tint = originalColor;
        }).SetId(stylizedShade);
    }
}
