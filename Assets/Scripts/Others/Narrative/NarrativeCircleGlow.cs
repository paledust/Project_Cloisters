using DG.Tweening;
using UnityEngine;

public class NarrativeCircleGlow : MonoBehaviour
{
    [SerializeField] private SpriteRenderer glowCircle;
    [SerializeField] private AnimationCurve fadeCurve;
    public void GlowOnCollision(float glowStrength)
    {
        glowCircle.DOKill();
        glowCircle.DOFade(glowStrength, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            glowCircle.DOFade(0, .5f).SetEase(fadeCurve);
        });
    }
}
