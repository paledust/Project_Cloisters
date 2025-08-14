using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    [SerializeField] private InputAction launchAction;
    [SerializeField] private Animation bounceAnimation;
    [SerializeField] private float coolDown = 0.15f;

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

        if (isfirstLaunch)
        {
            isfirstLaunch = false;
            EventHandler.Call_OnLaunchBall(Vector2.left * 10);
        }
        else
        {
            EventHandler.Call_OnLaunchBall(Vector2.left * 1);
        }
    }
    public void ResetLauncher()
    {
        isfirstLaunch = true;
        lastLaunchTime = Time.time;
    }
}
