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
        RespawnGame();
    }
    protected override void OnInteractionEnd()
    {
        EventHandler.E_OnBallDead -= RespawnGame;
        EventHandler.E_OnCollect -= OnCollect;
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
    }
}
