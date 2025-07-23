using UnityEngine;
using DG.Tweening;

public class Clickable_Bouncer : Basic_Clickable
{
    [SerializeField] private float cooldown = 0.1f;

    [Header("Feedback")]
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private SpriteRenderer blinkRender;
    [SerializeField] private float bounceSize = 1;
    [SerializeField] private float bounceSpeedBoost = 2;
    [SerializeField] private float bounceSpeedMulti = 2;

    private bool colliding = false;
    private float lastClickTime = 0;
    void OnEnable() => lastClickTime = -cooldown;

    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        if (Time.time > cooldown + lastClickTime)
        {
            base.OnClick(player, hitPos);
            lastClickTime = Time.time;
            blinkRender.DOKill();
            blinkRender.DOFade(1, 0.1f).OnComplete(() => blinkRender.DOFade(0, 0.05f));
            spriteRenderer.transform.DOKill();
            spriteRenderer.transform.DOPunchScale(bounceSize * Vector3.one, 0.1f, 1, 2).SetEase(Ease.OutQuad);
        }
    }

    void OnCollisionEnter(Collision collision)
    {

        if (!colliding && collision.impulse.magnitude > 0)
        {
            colliding = true;
            var bounceBall = collision.gameObject.GetComponent<BounceBall>();
            if (bounceBall != null)
            {
                float relVel = 0;
                if (Time.time < cooldown + lastClickTime)
                {
                    relVel += bounceSpeedBoost;
                }
                bounceBall.Bounce(Quaternion.Euler(0,0,Random.Range(-5, 5))*collision.impulse, relVel);
                spriteRenderer.transform.localScale = Vector3.one;
                spriteRenderer.transform.DOKill();
                spriteRenderer.transform.DOPunchScale(bounceSize * Vector3.one, 0.1f, 1, 2).SetEase(Ease.OutQuad);
            }
        }
    }
    void OnCollisionExit(Collision collision)
    {
        if (colliding)
            colliding = false;
    }
}
