using System.Collections;
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
        yield return coroutineCompleteStageBasic();
    }
}