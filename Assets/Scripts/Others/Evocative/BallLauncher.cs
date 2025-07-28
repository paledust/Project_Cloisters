using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class BallLauncher : MonoBehaviour
{
    [SerializeField] private InputAction launchAction;
    [SerializeField] private SpriteRenderer holdSprite;
    void OnEnable()
    {
        holdSprite.enabled = true;
        launchAction.Enable();
        launchAction.performed += OnLaunch;
    }
    void OnDisable()
    {
        holdSprite.enabled = false;
        launchAction.performed -= OnLaunch;
        launchAction.Disable();
    }
    void OnLaunch(InputAction.CallbackContext context)
    {
        EventHandler.Call_OnLaunchBall(Vector2.left * 10);
    }
}
