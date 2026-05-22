using System.Collections;
using Cinemachine;
using SimpleAudioSystem;
using UnityEngine;

public class EvocativeTube : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform eject;
    [SerializeField] private TubeEntranceTrigger tubeTrigger;
    [SerializeField] private float tubeTravelTime;
    [SerializeField] private float ejectSpeed;
    [SerializeField] private float ejectSpeedBoost;

    [Header("Presentation")]
    [SerializeField] private Animation ejectAnim;
    [SerializeField] private ParticleSystem vfxEject;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Audio")]
    [SerializeField] private string sfxTunnel;
    [SerializeField] private string sfxLaunch;

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
        AudioManager.Instance.PlaySFX(sfxTunnel, 1);
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
        ball.Bounce(eject.up * ejectSpeed, ejectSpeedBoost, 4);
        AudioManager.Instance.PlaySFX(sfxLaunch, 1);
        tubeTrigger.ResetTubeTrigger();
        isBallTravelling = false;
    }
}