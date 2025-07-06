using UnityEngine;
using DG.Tweening;

public class Bouncer : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private float bounceSize = 1;
    [SerializeField] private float bounceSpeedBoost = 2;
    void OnCollisionEnter(Collision collision)
    {
        Vector2 collideDir = collision.GetContact(0).normal;
        var bounceBall = collision.gameObject.GetComponent<Clickable_BounceBall>();
        if (bounceBall != null)
        {
            bounceBall.HandleBounce(collideDir, bounceSpeedBoost);
            spriteRenderer.transform.localScale = Vector3.one;
            spriteRenderer.transform.DOKill();
            spriteRenderer.transform.DOPunchScale(bounceSize * Vector3.one, 0.15f, 1, 2).SetEase(Ease.OutQuad);
        }
    }
}
