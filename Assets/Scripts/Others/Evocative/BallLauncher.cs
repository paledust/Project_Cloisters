using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    [SerializeField] private InputAction launchAction;
    [SerializeField] private Animation bounceAnimation;
    [SerializeField] private Bouncer bouncer;
    [SerializeField] private float launchSpeed = 10;
    [SerializeField] private float boostSpeed = 3;
    [SerializeField] private float maxSpeed = 13f;
    [SerializeField] private float coolDown = 0.15f;
    [SerializeField] private ParticleSystem p_launch;

    private bool isfirstLaunch;
    private float lastLaunchTime;

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
        if (Time.time - lastLaunchTime < coolDown)
            return;

        lastLaunchTime = Time.time;
        bounceAnimation.Play();
    }
    public void ResetLauncher()
    {
        isfirstLaunch = true;
        lastLaunchTime = Time.time;
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
            p_launch.transform.position = ball.transform.position;
            p_launch.Play();
            if (isfirstLaunch)
            {
                isfirstLaunch = false;
                ball.Launch(Vector2.right * launchSpeed);
            }
            else
            {
                if (ball.consistentSpeed < maxSpeed)
                {
                    ball.Bounce(Vector2.right, boostSpeed, 4f);
                }
                else
                {
                    ball.Bounce(Vector2.right, 0, 4f);
                }
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