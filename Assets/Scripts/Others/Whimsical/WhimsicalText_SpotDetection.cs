using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhimsicalText_SpotDetection : MonoBehaviour
{
    [SerializeField] private ChargeText parentText;
    [SerializeField] private float chargeSpeed = 0.1f;
    [SerializeField] private float dropSpeed = 0.5f;
    private float chargeProgress = 0;
    private bool isCharging = false;

    public void OnDetected()
    {
        isCharging = true;
        parentText.OnCharged();
    }
    void Update()
    {
        chargeProgress += (isCharging?chargeSpeed:-dropSpeed) * Time.deltaTime;
        chargeProgress = Mathf.Clamp01(chargeProgress);
        parentText.GetCharge(chargeProgress);
    }
    public void OnNotDetected()
    {
        isCharging = false;
        parentText.OnNotCharged();
    }
}
