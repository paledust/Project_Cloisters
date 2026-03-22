using UnityEngine;

public class WhimsicalTextController : MonoBehaviour
{
    [SerializeField] private ChargeText[] chargeTexts;
    [SerializeField] private AnimationCurve fullyChargeCurve;

    public int TotalTextCount => chargeTexts.Length;

    void Start()
    {
        Service.Shuffle(ref chargeTexts);
    }
    public void CompleteCharge()
    {
        this.enabled = false;
        foreach(var text in chargeTexts)
        {
            text.BlinkText(Random.Range(0.5f, 1f), fullyChargeCurve);
        }
    }
    public void PopoutAllText()
    {
        foreach(var text in chargeTexts)
        {
            text.PopoutText(Random.Range(0, 1f));
        }
    }
}
