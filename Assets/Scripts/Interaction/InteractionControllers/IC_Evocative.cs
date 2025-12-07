using System.Collections;
using Cinemachine;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.VFX;

public class IC_Evocative : IC_Basic
{
    [Header("Ball Launch")]
    [SerializeField] private Transform restartPos;
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private BounceBall bounceBall;

    [Header("Hit Feedback")]
    [SerializeField] private ParticleSystem bounceParticle;
    [SerializeField] private VisualEffect breakEffect;

    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Color[] backgroundColors;

    [Header("Goal")]
    [SerializeField] private bool isGoalBreakable = false;
    [SerializeField] private Bouncer_Goal goal;
    [SerializeField] private GameObject finalBoundry;
    [SerializeField] private PlayableDirector finalPlayable;
    [SerializeField] private PlatformCharge platformCharge;

    [Header("Boucner Manager")]
    [SerializeField] private Bouncer[] bouncers;

    [Header("Camera")]
    [SerializeField] private CinemachineVirtualCamera vc_cam;

    private int collectedCount = 0;
    private int backgroundIndex = 0;
    private CinemachineBasicMultiChannelPerlin perlineNoise;
    private CinemachineBrain brain;
    private Collectable[] collectables;
    private CoroutineExcuter cameraShaker;
    private CoroutineExcuter timeStutter;

    private const string WALL_TAG = "BounceWall";

    protected override void OnInteractionEnter()
    {
        EventHandler.E_OnBallFall += OnBallFall;
        EventHandler.E_OnCollect += OnCollect;
        EventHandler.E_OnHitGoal += OnHitGoal;
        EventHandler.E_OnBallHeavyBounce += OnBallHeavyBounce;
        EventHandler.E_OnGoalBreak += OnGoalBreak;

        BallRespawn();

        cameraShaker = new CoroutineExcuter(this);
        timeStutter = new CoroutineExcuter(this);
        perlineNoise = vc_cam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        brain = Camera.main.GetComponent<CinemachineBrain>();
        bouncers = interactionAssetsGroup.GetComponentsInChildren<Bouncer>();
        if (isGoalBreakable)
            goal.BecomeVulnerable();
        
        collectables = interactionAssetsGroup.GetComponentsInChildren<Collectable>(false);
    }
    protected override void OnInteractionEnd()
    {
        EventHandler.E_OnBallFall -= OnBallFall;
        EventHandler.E_OnCollect -= OnCollect;
        EventHandler.E_OnHitGoal -= OnHitGoal;
        EventHandler.E_OnBallHeavyBounce -= OnBallHeavyBounce;
        EventHandler.E_OnGoalBreak -= OnGoalBreak;
    }
    public void BallRespawn()
    {
        bounceBall.gameObject.SetActive(false);
        StartCoroutine(coroutineRespawn());
    }
    void OnBallHeavyBounce()
    {
        timeStutter.Excute(coroutineTimeStutter());
        cameraShaker.Excute(coroutineShakeCam(0.2f, 0.15f, true));
    }
    void OnBallFall()
    {
        BallRespawn();
    }
    void OnCollect(Collectable collectable)
    {
        bounceParticle.transform.position = collectable.transform.position;
        bounceParticle.Play();
        breakEffect.transform.position = collectable.transform.position;
        breakEffect.Play();

        Destroy(collectable.gameObject);
        collectedCount++;
        if (collectedCount >= collectables.Length && !isGoalBreakable)
        {
            StartCoroutine(coroutineAugmented());
        }
    }
    void OnHitGoal()
    {
        backgroundIndex++;
        backgroundIndex %= backgroundColors.Length;
        Color backgroundColor = backgroundColors[backgroundIndex];
        backgroundRenderer.DOKill();
        backgroundRenderer.DOColor(backgroundColor, 0.1f);
        cameraShaker.Excute(coroutineShakeCam(0.2f, bounceBall.m_isSuperCharge?0.2f:0.01f));
    }
    void OnGoalBreak()
    {
        finalBoundry.SetActive(true);
        finalPlayable.Play();
        bounceBall.BallFinal();

        foreach (var bouncer in bouncers)
        {
            if (bouncer != null && bouncer.tag != WALL_TAG)
                bouncer.GetComponent<Collider>().enabled = false;
        }
        cameraShaker.Excute(coroutineShakeCam(0.3f, 0.2f, true));
        EventHandler.Call_OnEndInteraction(this);
    }
    IEnumerator coroutineRespawn()
    {
        yield return new WaitForSeconds(1f);
        bounceBall.gameObject.SetActive(true);
        ballLauncher.ResetLauncher();
        bounceBall.ResetAtPos(restartPos.position);
    }
    IEnumerator coroutineAugmented()
    {
        isGoalBreakable = true;
        yield return new WaitForSeconds(1f);
        platformCharge.GlowOn();
    }
    IEnumerator coroutineTimeStutter()
    {
        perlineNoise.m_AmplitudeGain = 0.25f;
        CinemachineBrain brain = Camera.main.GetComponent<CinemachineBrain>();
        brain.m_IgnoreTimeScale = true;
        float originalTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        yield return new WaitForSecondsRealtime(0.1f);
        perlineNoise.m_AmplitudeGain = 0f;
        brain.m_IgnoreTimeScale = false;
        Time.timeScale = originalTimeScale;
    }
    IEnumerator coroutineShakeCam(float duration, float amplitude, bool ignoreTimescale = false)
    {
        if(ignoreTimescale)
            brain.m_IgnoreTimeScale = true;
        perlineNoise.m_AmplitudeGain = amplitude;
        yield return new WaitForSeconds(duration);
        perlineNoise.m_AmplitudeGain = 0f;
        brain.m_IgnoreTimeScale = false;
    }
}