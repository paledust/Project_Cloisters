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
        bounceBall.ResetAtPos(restartPos.position);
        ballLauncher.enabled = true;
    }
    void OnLaunchBall(Vector2 forceDir)
    {
        bounceBall.Launch(forceDir);
        ballLauncher.enabled = false;
    }
    void OnCollect(Collectable collectable)
    {
        bounceParticle.transform.position = collectable.transform.position;
        bounceParticle.Play();
        Destroy(collectable.gameObject);
    }
}
