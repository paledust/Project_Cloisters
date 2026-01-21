using SimpleAudioSystem;
using DG.Tweening;
using UnityEngine;

public class Clickable_Stylized : Basic_Clickable
{
    [SerializeField] private string hoverSFX;
    [SerializeField] private float hoverScalePunch = 1.1f;
    [SerializeField] private float hoverScaleDuration = 0.25f;

    private Vector3 originalScale;

    void OnEnable()
    {
        originalScale = transform.localScale;    
    }
    public override void OnHover(PlayerController player)
    {
        base.OnHover(player);
        StylizedDrumController.Instance.QueueBeat(hoverSFX, 1f);
        transform.DOKill();
        transform.localScale = originalScale;
        transform.DOPunchScale(Vector3.one * hoverScalePunch, 0.25f, 2);
    }
}
