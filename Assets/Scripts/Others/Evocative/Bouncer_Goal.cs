using UnityEngine;

public class Bouncer_Goal : MonoBehaviour
{

    [SerializeField] private Bouncer bouncer;
    [SerializeField] private SpriteRenderer chargeSprite;
    [SerializeField] private PerRendererGoalRender goalRender;
    [SerializeField] private float minhitStep = 0.2f;
    [SerializeField] private GameObject[] cracks;

    [Header("VFX")]
    [SerializeField] private ParticleSystem p_break;

    private int criticalHitCount;
    private float lastHitTime = 0f;

    void Awake()
    {
        criticalHitCount = 0;
        bouncer = GetComponent<Bouncer>();
        bouncer.onBounce += BounceHandle;
        lastHitTime = Time.time;
    }
    void OnDestroy()
    {
        bouncer.onBounce -= BounceHandle;
    }
    void BounceHandle(BounceBall bounceBall)
    {
        goalRender.ImpulseRapidNoise(2f, 0.1f, 1f);
        if (Time.time - lastHitTime < minhitStep)
        {
            return;
        }
        lastHitTime = Time.time;

        if(bounceBall.m_isSuperCharge)
        {
            if (criticalHitCount < cracks.Length)
            {
                cracks[criticalHitCount].SetActive(true);
                var dir = bounceBall.transform.position - transform.position;
                float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
                p_break.transform.rotation = Quaternion.Euler(0, 0, angle);
                p_break.Play();
            }
            else
            {
                EventHandler.Call_OnGoalBreak();    
            }
            criticalHitCount++;
        }

        EventHandler.Call_OnHitGoal();
    }
    public void BecomeVulnerable()
    {
        chargeSprite.gameObject.SetActive(true);
    }
}