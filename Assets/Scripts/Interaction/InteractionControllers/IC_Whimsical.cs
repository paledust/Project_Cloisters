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
    void CountText()
    {
        count ++;
        if(count >= textController.TotalTextCount)
        {
            EventHandler.Call_OnEndInteraction(this);
            StartCoroutine(coroutineEnd());
        }
    }
    IEnumerator coroutineEnd()
    {
        yield return new WaitForSeconds(0.5f);
        textController.CompleteCharge();
        crystal.FadeIdleAngularSpeed(150, 4f);
        yield return new WaitForSeconds(3f);
        textController.PopoutAllText();
        yield return new WaitForSeconds(1f);
        endDirector.Play();
        yield return new WaitForSeconds(2f);
        EventHandler.Call_OnInteractionUnreachable(this);
    }
}
