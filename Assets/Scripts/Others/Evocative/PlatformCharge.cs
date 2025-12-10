using UnityEngine;

public class PlatformCharge : MonoBehaviour
{
    [SerializeField] private BallLauncher ballLauncher;
    [SerializeField] private GameObject glowEffect;

    private Bouncer bouncer;

    void Awake()
    {
        bouncer = GetComponent<Bouncer>();
    }
    public void GlowOn()
    {
        ballLauncher.SuperCharge();
        ballLauncher.onLaunchBall += ChargeBall;
        bouncer.onBounce += ChargeBall;
        glowEffect.SetActive(true);
    }
    void ChargeBall(BounceBall ball)
    {
        ballLauncher.onLaunchBall -= ChargeBall;
        bouncer.onBounce -= ChargeBall;
        ball.ChargeBounceBall();
    }
}