using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IC_Whimsical : IC_Basic
{
    [SerializeField] private WhimsicalTextController textController;
    [SerializeField] int count = 0;
    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
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
        if(count >= textController.TotalTextCount)
        {
            textController.CompleteCharge();
            EventHandler.Call_OnEndInteraction(this);
        }
    }
}
