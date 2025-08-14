using UnityEngine;

public class IC_Evocative : IC_Basic
{
    [SerializeField] private Transform restartPos;
    [SerializeField] private BounceBall bounceBall;
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private ParticleSystem bounceParticle;

    protected override void OnInteractionEnter()
    {
        EventHandler.E_OnBallDead += RespawnGame;
        EventHandler.E_OnCollect += OnCollect;
        EventHandler.E_OnLaunchBall += OnLaunchBall;
        RespawnGame();
    }
    protected override void OnInteractionEnd()
    {
        EventHandler.E_OnBallDead -= RespawnGame;
        EventHandler.E_OnCollect -= OnCollect;
        EventHandler.E_OnLaunchBall -= OnLaunchBall;
    }
    public void RespawnGame()
    {
        ballLauncher.ResetLauncher();
        bounceBall.ResetAtPos(restartPos.position);
    }
    void OnLaunchBall(Vector2 forceDir)
    {
        Vector2 diff = ballLauncher.transform.position - bounceBall.transform.position;
        if (Mathf.Abs(diff.y) <= 2 && Mathf.Abs(diff.x) <= 1f)
            bounceBall.Launch(forceDir);
    }
    void OnCollect(Collectable collectable)
    {
        bounceParticle.transform.position = collectable.transform.position;
        bounceParticle.Play();
        Destroy(collectable.gameObject);
    }
}
