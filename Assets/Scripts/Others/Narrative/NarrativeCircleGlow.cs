using DG.Tweening;
using UnityEngine;

public class NarrativeCircleGlow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer glowCircle;
    [SerializeField] private AnimationCurve fadeCurve;
    [SerializeField] private float glowFadeDuration = 0.5f;
    public void GlowOnCollision(float glowStrength)
    {
        glowCircle.DOKill();
        glowCircle.DOFade(glowStrength, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            glowCircle.DOFade(0, glowFadeDuration).SetEase(fadeCurve);
        });
    }
}
