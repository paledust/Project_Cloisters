using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EStage_ConnectAllShape : ExperimentalStageBasic
{
    public override bool IsDone()=>true;
    public override void CompleteStage()
    {
        StartCoroutine(coroutineCompleteStage());
    }
    IEnumerator coroutineCompleteStage()
    {
        yield return new WaitForSeconds(1f);
        experimentalController.BreakConnectionAndPopText();
    }
}