using UnityEngine;

public class WhimsicalTextController : MonoBehaviour
{
    [SerializeField] private ChargeText[] chargeTexts;
    [SerializeField] private AnimationCurve fullyChargeCurve;

    [SerializeField, ShowOnly] private float totalChargeValue = 0;

    public int TotalTextCount => chargeTexts.Length;

    private const float MAX_CHARGE = 3f;

    void Start()
    {
        Service.Shuffle(ref chargeTexts);
        foreach(var text in chargeTexts)
        {
            text.GetCharge(totalChargeValue);
        }
        int index = Random.Range(0, chargeTexts.Length);
    }
    public void CompleteCharge()
    {
        this.enabled = false;
        foreach(var text in chargeTexts)
        {
            text.StayCharged(Random.Range(0.5f, 1f), fullyChargeCurve);
        }
        totalChargeValue = MAX_CHARGE;
    }
    public void PopoutAllText()
    {
        foreach(var text in chargeTexts)
        {
            text.PopoutText(Random.Range(0, 1f));
        }
    }
}
