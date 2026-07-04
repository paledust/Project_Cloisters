using DG.Tweening;
using UnityEngine;

public class CollectableText : MonoBehaviour
{
    [SerializeField] private char collectKey;
    [SerializeField] private Animation textAnimation;
    [SerializeField] private ParticleSystem P_textPop;

    public char m_collectKey=>collectKey;

    public void CollectText(float delay)
    {
        transform.DOPunchScale(Vector3.one*1.1f, 0.25f, 1, 1)
        .SetDelay(delay).OnComplete(()=>{
            EventHandler.Call_OnCollectExperimentalText(this);
        });
    }
    public void PopText()
    {
        P_textPop.Play();
        textAnimation.Play();
        transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack, 10);
    }
}