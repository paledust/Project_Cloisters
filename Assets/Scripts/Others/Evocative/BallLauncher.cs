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

    private bool ballReady = false;
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
        if(ballReady)
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
        ballReady = true;
    }
    public void AE_ResetCanBounce()
    {
        bouncer.SwitchCanBounce(true);
    }
    void OnTriggerEnter(Collider other)
    {
        var ball = other.GetComponent<BounceBall>();
        if (ball != null)
        {
            bouncer.PlayBounceFeedback();
            onLaunchBall?.Invoke(ball);

            if (ballReady)
            {
                ballReady = false;
                ball.Launch(Vector2.right * (launchSpeed + (isSuperCharge ? boostSpeed * 0.5f : 0)), 2);
                AudioManager.Instance.PlaySFX(sfxLaunch.AudioKey, 1);
            }
        }
    }
}