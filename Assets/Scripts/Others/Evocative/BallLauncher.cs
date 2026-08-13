using System;
using SimpleAudioSystem;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    [SerializeField] private InputAction launchAction;
    [SerializeField] private Animation bounceAnimation;
    [SerializeField] private Bouncer bouncer;
    [SerializeField] private float launchSpeed = 10;
    [SerializeField] private float boostSpeed = 3;
    [SerializeField] private AudioData_SO sfxLaunch;
    [SerializeField] private AudioData_SO sfxUpgrade;

    private bool ballLaunched;
    private bool isSuperCharge = false;
    public Action<BounceBall> onLaunchBall;

    void OnEnable()
    {
        launchAction.Enable();
        launchAction.performed += OnLaunch;
    }
    void OnDisable()
    {
        launchAction.performed -= OnLaunch;
        launchAction.Disable();
    }
    void OnLaunch(InputAction.CallbackContext context)
    {
        if(!ballLaunched)
            return;
        bounceAnimation.Play();
    }
    public void SuperCharge()
    {
        isSuperCharge = true;
        bouncer.ChangeBounceParam(0, 4f);
        AudioManager.Instance.PlaySFX(sfxUpgrade.AudioKey, 1);
    }
    public void ResetLauncher()
    {
        ballLaunched = true;
    }
    public void AE_ResetCanBounce()
    {
        bouncer.SwitchCanBounce(true);
    }
    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<BounceBall>();
        if (ball != null && !bouncer.m_colliding)
        {
            bouncer.SwitchCanBounce(false);
            bouncer.PlayBounceFeedback();

            onLaunchBall?.Invoke(ball);

            if (ballLaunched)
            {
                ballLaunched = false;
                AudioManager.Instance.PlaySFX(sfxLaunch.AudioKey, 1);
                ball.Launch(Vector2.right * (launchSpeed + (isSuperCharge ? boostSpeed * 0.5f : 0)), 2);
            }
        }
    }
    void OnTriggerExit(Collider other)
    {
        var ball = other.GetComponent<BounceBall>();
        if (ball != null)
        {
            bouncer.SwitchCanBounce(true);
        }
    }
}