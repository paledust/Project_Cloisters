using UnityEngine;

public class WhimsicalText_SpotDetection : MonoBehaviour
{
    [SerializeField] private ChargeText parentText;
    [SerializeField] private float chargeSpeed = 0.1f;
    [SerializeField] private float dropSpeed = 0.5f;
    private float chargeProgress = 0;
    private bool charged = false;
    private WhimsicalTextSpoter whimsicalTextSpoter;

    public void OnDetected(WhimsicalTextSpoter whimsicalTextSpoter)
    {
        this.whimsicalTextSpoter = whimsicalTextSpoter;
    }
    void Update()
    {
        chargeProgress += (whimsicalTextSpoter?chargeSpeed:-dropSpeed) * Time.deltaTime;
        chargeProgress = Mathf.Clamp01(chargeProgress);
        parentText.GetCharge(chargeProgress);

        if(chargeProgress >= 1)
        {
            if(!charged)
            {
                this.charged = true;
                this.enabled = false;
                parentText.FullyCharged();
                whimsicalTextSpoter.OnConsumed();
                EventHandler.Call_OnChargeText(true);

                return;
            }
        }
    }
    public void OnNotDetected()
    {
        whimsicalTextSpoter = null;
    }
}
