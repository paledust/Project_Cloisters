using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MirrorDiamond : MonoBehaviour
{
    [SerializeField] private float focusDuration = 1f;
    [SerializeField] private ParticleSystem rippleEffect;

    private bool isFocused = false;
    private float focusTimer = 0f;
    private Animator animator_diamond;
    private Collider hitbox;

    private const string ACTIVATE_TRIGGER = "activate";
    private const string FOUND_TRIGGER = "found";
    private const string FOCUS_BOOL = "isFocused";

    void Awake()
    {
        animator_diamond = GetComponent<Animator>();
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
    }
    public void ActivateDiamond()
    {
        animator_diamond.SetTrigger(ACTIVATE_TRIGGER);
        hitbox.enabled = true;
    }
    void Update()
    {
        if(isFocused)
        {
            focusTimer += Time.deltaTime;
            if(focusTimer >= focusDuration)
            {
                EventHandler.Call_OnMirrorDiamondFound();
                animator_diamond.SetTrigger(FOUND_TRIGGER);
            }
        }
    }
    public void OnFocused()
    {
        animator_diamond.SetBool(FOCUS_BOOL, true);
        focusTimer = 0f;
        isFocused = true;
    }
    public void OnExitFocus()
    {
        animator_diamond.SetBool(FOCUS_BOOL, false);
        focusTimer = 0f;
        isFocused = false;
    }
    public void AE_Ripple()
    {
        if (rippleEffect != null)
        {
            rippleEffect.Emit(1);
        }
    }
}
