using UnityEngine;
using DG.Tweening;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer blinkRender;
    [SerializeField] private float bounceSize = 1;
    [SerializeField] private float bounceSpeedBoost = 2;
    private bool colliding = false;
    void OnCollisionEnter(Collision collision)
    {
        if (!colliding && collision.impulse.magnitude > 0)
        {
            colliding = true;
            var bounceBall = collision.gameObject.GetComponent<BounceBall>();
            if (bounceBall != null)
            {
                bounceBall.Bounce(collision.impulse, bounceSpeedBoost);
                spriteRenderer.transform.localScale = Vector3.one;
                spriteRenderer.transform.DOKill();
                spriteRenderer.transform.DOPunchScale(bounceSize * Vector3.one, 0.1f, 1, 2).SetEase(Ease.OutQuad);

                blinkRender.DOKill();
                blinkRender.DOFade(1, 0.1f).OnComplete(()=>blinkRender.DOFade(0,0.2f));
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (colliding)
            colliding = false;
    }
}
