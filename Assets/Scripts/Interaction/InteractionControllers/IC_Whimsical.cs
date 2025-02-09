using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Whimsical : IC_Basic
{
    [SerializeField] private List<ChargeText> chargeTexts;
    [SerializeField] private AnimationCurve fullyChargeCurve;
    [SerializeField] int count = 0;
    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        EventHandler.E_OnChargeText += CountText;
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        EventHandler.E_OnChargeText -= CountText;
    }
    void CountText(bool isCharged)
    {
        count += isCharged?1:-1;
        if(count >= chargeTexts.Count)
        {
            foreach(var text in chargeTexts)
            {
                text.StayCharged(Random.Range(0.5f, 1f), fullyChargeCurve);
            }
            EventHandler.Call_OnEndInteraction(this);
        }
    }
}
