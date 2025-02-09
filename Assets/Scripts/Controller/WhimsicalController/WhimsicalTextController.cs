using UnityEngine;

public class WhimsicalTextController : MonoBehaviour
{
    [SerializeField] private ChargeText[] chargeTexts;
    [SerializeField] private Clickable_ObjectRotator diamond;
    [SerializeField] private AnimationCurve fullyChargeCurve;
    [SerializeField] private float minimumAngularSpeed = 100;
    [SerializeField] private float chargeSpeed = 0.1f;
    [SerializeField] private float dropSpeed = 0.5f;

    [SerializeField, ShowOnly] private float totalChargeValue = 0;

    public int TotalTextCount => chargeTexts.Length;

    private const float MAX_CHARGE = 3f;

    void Start()
    {
        Service.Shuffle(ref chargeTexts);
        foreach(var text in chargeTexts)
        {
            text.GetCharge(totalChargeValue);
            text.phase = Random.Range(0f, MAX_CHARGE);
        }
        int index = Random.Range(0, chargeTexts.Length);

        chargeTexts[index].phase = 0;
        chargeTexts[(index+1)%chargeTexts.Length].phase = 3;
    }
    void Update()
    {
        if(Mathf.Abs(diamond.m_angularSpeed) > minimumAngularSpeed)
        {
            totalChargeValue += chargeSpeed * Time.deltaTime;
        }
        else
        {
            totalChargeValue -= dropSpeed * Time.deltaTime;
            totalChargeValue = Mathf.Max(0, totalChargeValue);
        }
        foreach(var text in chargeTexts)
        {
            text.GetCharge(totalChargeValue);
        }
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
}
