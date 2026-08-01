using DG.Tweening;
using UnityEngine;

public class WhimsicalSpotCircle : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    private Tween circleScaleTween;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    public void ShowUp(float targetScale)
    {
        if(circleScaleTween!=null)
            circleScaleTween.Kill();
        circleScaleTween = transform.DOScale(1.2f, 0.25f).SetEase(Ease.OutBack);
    }
    public void HideOut(float targetScale)
    {
        if(circleScaleTween!=null)
            circleScaleTween.Kill();
        circleScaleTween = transform.DOScale(targetScale, 0.25f).SetEase(Ease.InBack);
    }
}
