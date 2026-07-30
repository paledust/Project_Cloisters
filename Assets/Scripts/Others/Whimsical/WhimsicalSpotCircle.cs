using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class WhimsicalSpotCircle : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Tween transformScaler;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ShowUp(float targetScale)
    {
        if(transformScaler!=null)
        {
            transformScaler.Kill();
        }
        transformScaler = transform.DOScale(1.2f, 0.25f).SetEase(Ease.OutBack);
    }
    public void HideOut(float targetScale)
    {
        if(transformScaler!=null)
        {
            transformScaler.Kill();
        }
        transformScaler = transform.DOScale(targetScale, 0.25f).SetEase(Ease.InBack);
    }
}
