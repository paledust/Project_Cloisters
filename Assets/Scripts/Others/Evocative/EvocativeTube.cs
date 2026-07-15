using System.Collections;
using Cinemachine;
using DG.Tweening;
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
    [SerializeField] private GameObject entranceCircleIndicator;

    [Header("Tube Parent")]
    [SerializeField] private Transform tubeRoot;
    [SerializeField] private float shakeStrength;
    [SerializeField] private int shakeVibration = 10;

    [Header("Presentation")]
    [SerializeField] private Animation ejectAnim;
    [SerializeField] private ParticleSystem vfxEject;
    [SerializeField] private CinemachineImpulseSource impulseSource;

    [Header("Audio")]
    [SerializeField] private AudioData_SO sfxTunnel;
    [SerializeField] private AudioData_SO sfxLaunch;

    private bool isBallTravelling;
    private Tween tubeShakeTween;
    private Vector3 tubeLocalPos;

    void Start()
    {
        tubeLocalPos = tubeRoot.localPosition;
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
    public void StartTubeShake()
    {
        if(tubeShakeTween!=null)
            tubeShakeTween.Kill();
        tubeShakeTween = tubeRoot.DOShakePosition(2f, shakeStrength, shakeVibration).SetLoops(-1);
    }
    public void StopTubeShake()
    {
        if(tubeShakeTween!=null)
            tubeShakeTween.Kill();
        tubeShakeTween = tubeRoot.DOLocalMove(tubeLocalPos, 1f);
    }
    IEnumerator coroutineTubeTravel(BounceBall ball)
    {
        entranceCircleIndicator.SetActive(true);
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
        AudioManager.Instance.PlaySFX(sfxTunnel.AudioKey, 1);
        tubeRoot.DOShakePosition(.25f, shakeStrength, shakeVibration*2);

        ball.gameObject.SetActive(false);
        yield return new WaitForSeconds(.25f);
        ejectAnim.Play();
        yield return new WaitForSeconds(tubeTravelTime);
        impulseSource.GenerateImpulse();
        ball.transform.position = eject.position;
        ball.gameObject.SetActive(true);
        ball.WakePhysics();
        ball.GlowBall();
        ball.Bounce(eject.up * ejectSpeed, ejectSpeedBoost, 4);
        AudioManager.Instance.PlaySFX(sfxLaunch.AudioKey, 1);
        tubeTrigger.ResetTubeTrigger();
        isBallTravelling = false;
    }
}