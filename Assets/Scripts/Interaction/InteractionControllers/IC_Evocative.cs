using DG.Tweening;
using UnityEngine;

public class IC_Evocative : IC_Basic
{
    [Header("Ball Launch")]
    [SerializeField] private Transform restartPos;
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private BounceBall bounceBall;

    [Header("Hit Feedback")]
    [SerializeField] private ParticleSystem bounceParticle;

    [Header("Background")]
    [SerializeField] private SpriteRenderer backgroundRenderer;
    [SerializeField] private Color[] backgroundColors;

    [Header("Goal")]
    [SerializeField] private Collectable[] collectables;
    [SerializeField] private GameObject finalText;
    private int collectedCount = 0;
    private int backgroundIndex = 0;

    protected override void OnInteractionEnter()
    {
        EventHandler.E_OnBallDead += RespawnGame;
        EventHandler.E_OnCollect += OnCollect;
        EventHandler.E_OnHitGoal += OnHitGoal;

        RespawnGame();
    }
    protected override void OnInteractionEnd()
    {
        EventHandler.E_OnBallDead -= RespawnGame;
        EventHandler.E_OnCollect -= OnCollect;
        EventHandler.E_OnHitGoal -= OnHitGoal;

    }
    public void RespawnGame()
    {
        ballLauncher.ResetLauncher();
        bounceBall.ResetAtPos(restartPos.position);
    }
    void OnCollect(Collectable collectable)
    {
        bounceParticle.transform.position = collectable.transform.position;
        bounceParticle.Play();
        Destroy(collectable.gameObject);
        collectedCount++;
    }
    void OnHitGoal()
    {
        backgroundIndex++;
        backgroundIndex %= backgroundColors.Length;
        Color backgroundColor = backgroundColors[backgroundIndex];
        backgroundRenderer.DOKill();
        backgroundRenderer.DOColor(backgroundColor, 0.1f);
        if (collectedCount >= collectables.Length)
        {
            finalText.SetActive(true);
            EventHandler.Call_OnEndInteraction(this);
        }
    }
}