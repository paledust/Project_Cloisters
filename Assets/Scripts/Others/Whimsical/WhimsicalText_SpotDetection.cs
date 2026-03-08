using System.Collections;
using UnityEngine;

public class WhimsicalText_SpotDetection : MonoBehaviour
{
    [SerializeField] private ChargeText parentText;
    [SerializeField] private float chargeSpeed = 0.1f;
    [SerializeField] private float dropSpeed = 0.5f;

    [Header("Additional text lighten")]
    [SerializeField] private ChargeText[] additionalChargedTexts;
    private float chargeProgress = 0;
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
            this.enabled = false;
            whimsicalTextSpoter.OnConsumed();
            GetComponent<Collider>().enabled = false;
            StartCoroutine(coroutineChargeNearbyText());

            return;
        }
    }
    public void OnNotDetected()
    {
        whimsicalTextSpoter = null;
    }
    IEnumerator coroutineChargeNearbyText()
    {
        foreach(var text in additionalChargedTexts)
        {
            yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
            text.GetCharge(1);
        }
        Destroy(gameObject);
    }
}
