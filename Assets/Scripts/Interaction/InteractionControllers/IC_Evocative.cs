using System.Collections;
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
    [SerializeField] private bool isGoalVulnerable = false;
    [SerializeField] private int requireHitToBreak = 4;
    [SerializeField] private Bouncer_Goal goal;
    [SerializeField] private Collectable[] collectables;
    [SerializeField] private PlayableDirector finalPlayable;

    [Header("Restart")]
    [SerializeField] private VFX_FallIntoHole vfx_fall;
    [SerializeField] private Animation animation_light;

    [Header("Boucner Manager")]
    [SerializeField] private Bouncer[] bouncers;

    private int hitTime = 0;
    private int collectedCount = 0;
    private int backgroundIndex = 0;

    private const string WALL_TAG = "BounceWall";

    protected override void OnInteractionEnter()
    {
        EventHandler.E_OnBallDead += RespawnGame;
        EventHandler.E_OnCollect += OnCollect;
        EventHandler.E_OnHitGoal += OnHitGoal;

        RespawnGame();

        bouncers = interactionAssetsGroup.GetComponentsInChildren<Bouncer>();
        if (isGoalVulnerable)
        {
            goal.BecomeVulnerable();
        }
    }
    protected override void OnInteractionEnd()
    {
        EventHandler.E_OnBallDead -= RespawnGame;
        EventHandler.E_OnCollect -= OnCollect;
        EventHandler.E_OnHitGoal -= OnHitGoal;

    }
    public void RespawnGame()
    {
        bounceBall.gameObject.SetActive(false);
        animation_light.Play();
        vfx_fall.PlayFallVFX();
        StartCoroutine(coroutineRespawn());
    }
    void OnCollect(Collectable collectable)
    {
        bounceParticle.transform.position = collectable.transform.position;
        bounceParticle.Play();
        breakEffect.transform.position = collectable.transform.position;
        breakEffect.Play();

        Destroy(collectable.gameObject);
        collectedCount++;
        if (collectedCount >= collectables.Length && !isGoalVulnerable)
        {
            isGoalVulnerable = true;
            goal.BecomeVulnerable();
        }
    }
    void OnHitGoal()
    {
        backgroundIndex++;
        backgroundIndex %= backgroundColors.Length;
        Color backgroundColor = backgroundColors[backgroundIndex];
        backgroundRenderer.DOKill();
        backgroundRenderer.DOColor(backgroundColor, 0.1f);
        if (isGoalVulnerable)
        {
            hitTime++;
            if (hitTime >= requireHitToBreak)
            {
                finalPlayable.Play();
                bounceBall.BallFinal();

                foreach (var bouncer in bouncers)
                {
                    if (bouncer != null && bouncer.tag != WALL_TAG)
                        bouncer.GetComponent<Collider>().enabled = false;
                }

                EventHandler.Call_OnEndInteraction(this);
            }
        }
    }
    IEnumerator coroutineRespawn()
    {
        yield return new WaitForSeconds(1f);
        bounceBall.gameObject.SetActive(true);
        ballLauncher.ResetLauncher();
        bounceBall.ResetAtPos(restartPos.position);
    }
}