using System.Collections;
using Cinemachine;
using UnityEngine;

public class EvocativeTube : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform eject;
    [SerializeField] private TubeEntranceTrigger tubeTrigger;
    [SerializeField] private float tubeTravelTime;
    [SerializeField] private float ejectSpeed;

    [Header("Presentation")]
    [SerializeField] private Animation ejectAnim;
    [SerializeField] private ParticleSystem vfxEject;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    private bool isBallTravelling;

    void Start()
    {
        tubeTrigger.InitTrigger(this);
    }
    public void EnterTube(BounceBall ball)
    {
        if (!isBallTravelling)
        {
            StartCoroutine(coroutineTubeTravel(ball));
        }
    }
    public void AE_PlayEjectParticle()
    {
        vfxEject.Play();
    }
    IEnumerator coroutineTubeTravel(BounceBall ball)
    {
        isBallTravelling = true;
        ball.PhysicsSleep();
        Vector3 initPos = ball.transform.position;
        float speed = ball.m_currentSpeed;
        float dist = Vector3.Distance(initPos, start.position);
        float enterTime = dist / speed;
        yield return new WaitForLoop(enterTime, (t) =>
        {
            ball.transform.position = Vector3.Lerp(initPos, start.position, t);
        });
        ejectAnim.Play();
        ball.gameObject.SetActive(false);
        yield return new WaitForSeconds(tubeTravelTime);
        impulseSource.GenerateImpulse();
        ball.transform.position = eject.position;
        ball.gameObject.SetActive(true);
        ball.WakePhysics();
        ball.GlowBall();
        ball.Bounce(eject.up * ejectSpeed, 0, 4);
        tubeTrigger.ResetTubeTrigger();
        isBallTravelling = false;
    }
}