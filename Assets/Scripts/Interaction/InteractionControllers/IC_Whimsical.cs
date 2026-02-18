using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Whimsical : IC_Basic
{
    [SerializeField] private Clickable_ObjectRotator crystal;
    [SerializeField] private WhimsicalTextController textController;
    [SerializeField] private int count = 0;
    [SerializeField] private PlayableDirector endDirector;
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
            StartCoroutine(coroutineEnd());
        }
    }
    IEnumerator coroutineEnd()
    {
        crystal.FadeIdleAngularSpeed(100, 5f);
        yield return new WaitForSeconds(3f);
        textController.PopoutAllText();
        yield return new WaitForSeconds(1f);
        endDirector.Play();
        yield return new WaitForSeconds(2f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
}
