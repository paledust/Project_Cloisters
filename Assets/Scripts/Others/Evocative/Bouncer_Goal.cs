using SimpleAudioSystem;
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
    [SerializeField] private ParticleSystem p_emit_loop;
    [SerializeField] private ParticleSystem p_ring_critical;
    [SerializeField] private ParticleSystem p_ring_undamaged;
    [SerializeField] private SpriteRenderer glow;
    [SerializeField] private Color glowColor;

    [Header("SFX")]
    [SerializeField] private AudioData_SO sfxNormalBounce;
    [SerializeField] private AudioData_SO[] sfxBreakings;

    private int criticalHitCount;
    private float lastHitTime = 0f;

    void Awake()
    {
        criticalHitCount = 0;
        bouncer = GetComponent<Bouncer>();
        bouncer.onPreBounce += PreBounceHandle;
        lastHitTime = Time.time;
    }
    void OnDestroy()
    {
        bouncer.onPreBounce -= PreBounceHandle;
    }
    void PreBounceHandle(BounceBall bounceBall)
    {
        goalRender.ImpulseRapidNoise(2f, 0.1f, 1f);
        if (Time.time - lastHitTime < minhitStep)
        {
            return;
        }
        lastHitTime = Time.time;

        var dir = bounceBall.transform.position - transform.position;
        float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
        p_break.transform.rotation = Quaternion.Euler(0, 0, angle);
        p_break.Play();
        
        if(bounceBall.m_isSuperCharge)
        {
            if (criticalHitCount < cracks.Length)
            {
                cracks[criticalHitCount].SetActive(true);
                p_ring_critical.Play(true);
                AudioManager.Instance.PlaySFX(sfxNormalBounce?.AudioKey, .25f);
                AudioManager.Instance.PlaySFX(sfxBreakings[criticalHitCount]?.AudioKey, 1);
            }
            else
            {
                EventHandler.Call_OnGoalBreak();    
            }

            if(!p_emit_loop.isPlaying)
                p_emit_loop.Play();
                
            var emission = p_emit_loop.emission;
            emission.rateOverTimeMultiplier = Mathf.Lerp(7, 30, criticalHitCount/2f);

            criticalHitCount++;

            glow.color = Color.Lerp(Color.black, glowColor, criticalHitCount/3f);
            goalRender.StackUpNoise(criticalHitCount);

            if(criticalHitCount > cracks.Length)
            {
                bouncer.SwitchCanBounce(false);
                bouncer.GetComponent<Collider>().enabled = false;
                bounceBall.Bounce(-bounceBall.GetComponent<Rigidbody>().velocity, 0, 2f, AttributeModifyType.Add);
            }
        }
        else
        {
            AudioManager.Instance.PlaySFX(sfxNormalBounce?.AudioKey, 1);
            p_ring_undamaged.Play();
        }

        EventHandler.Call_OnHitGoal(bounceBall.m_isSuperCharge);
    }
    public void BecomeVulnerable()
    {
        chargeSprite.gameObject.SetActive(true);
    }
}