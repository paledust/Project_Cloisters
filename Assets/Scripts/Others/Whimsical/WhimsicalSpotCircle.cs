using DG.Tweening;
using UnityEngine;

public class WhimsicalSpotCircle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    void Awake()
    {
        transform.localScale = Vector3.zero;
        spriteRenderer.color = new Color(1, 1, 1, 0);
    }
    public void ShowUp()
    {
        transform.DOKill();
        transform.DOScale(1, 0.25f).SetEase(Ease.OutBack);
        spriteRenderer.DOKill();
        spriteRenderer.DOFade(1, 0.25f);
    }
    public void HideOut()
    {
        transform.DOKill();
        transform.DOScale(0, 0.5f).SetEase(Ease.InBack).OnComplete(()=>spriteRenderer.color = new Color(1, 1, 1, 0));
        // spriteRenderer.DOKill();
        // spriteRenderer.DOFade(0, 0.25f);
    }
    public void OnDetect()
    {
        spriteRenderer.transform.DOKill();
        spriteRenderer.transform.DOScale(1.7f, 0.2f).SetEase(Ease.OutQuad);
    }
    public void OnExitDetect()
    {
        spriteRenderer.transform.DOKill();
        spriteRenderer.transform.DOScale(1.3f, 0.2f).SetEase(Ease.Linear);
    }
}