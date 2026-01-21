using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Hoverable_Glow : MonoBehaviour
{
    [SerializeField] private Basic_Clickable targetClickable;
    [SerializeField] private GlowObjectCmd glowObjectCmd;
    [SerializeField, ColorUsage(true, true)] private Color glowColor;
    
    void OnEnable()
    {
        targetClickable.onHover += OnHover;
        targetClickable.onExitHover += OnExitHover;
    }
    void OnDisable()
    {
        targetClickable.onHover -= OnHover;
        targetClickable.onExitHover -= OnExitHover;
    }
    public void OnHover(PlayerController player)
    {
        DOTween.Kill(this);
        DOTween.To(()=>glowObjectCmd.GlowColor, x=>glowObjectCmd.GlowColor = x, glowColor, 0.2f);
    }
    public void OnExitHover()
    {
        DOTween.Kill(this);
        DOTween.To(()=>glowObjectCmd.GlowColor, x=>glowObjectCmd.GlowColor = x, new Color(glowColor.r, glowColor.g, glowColor.b, 0), 0.2f);
    }
}