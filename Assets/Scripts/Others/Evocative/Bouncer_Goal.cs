using UnityEngine;

public class Bouncer_Goal : MonoBehaviour
{

    [SerializeField] private Bouncer bouncer;
    [SerializeField] private SpriteRenderer chargeSprite;
    [SerializeField] private PerRendererGoalRender goalRender;
    [SerializeField] private float minhitStep = 0.2f;
    [SerializeField] private GameObject[] cracks;

    private int criticalHitCount;
    private float lastHitTime = 0f;
    private bool isVulnerable = false;

    void Awake()
    {
        criticalHitCount = 0;
        isVulnerable = false;
        bouncer = GetComponent<Bouncer>();
        bouncer.onBounce += BounceHandle;
        lastHitTime = Time.time;
    }
    void BounceHandle(BounceBall bounceBall)
    {
        goalRender.ImpulseRapidNoise(2f, 0.1f, 1f);
        if (Time.time - lastHitTime < minhitStep)
        {
            return;
        }
        lastHitTime = Time.time;
        EventHandler.Call_OnHitGoal();

        if (isVulnerable)
        {
            if (criticalHitCount < cracks.Length)
            {
                cracks[criticalHitCount].SetActive(true);
            }
            criticalHitCount++;
        }
    }
    public void BecomeVulnerable()
    {
        isVulnerable = true;
        chargeSprite.gameObject.SetActive(true);
    }
}