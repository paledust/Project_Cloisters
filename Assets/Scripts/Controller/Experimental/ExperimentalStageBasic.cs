using System.Collections;
using UnityEngine;

public abstract class ExperimentalStageBasic : MonoBehaviour
{
    [SerializeField] protected IC_Experimental experimentalController;
    public virtual void CompleteStage(){}
    public abstract bool IsDone();
    protected IEnumerator coroutineCompleteStageBasic()
    {
        experimentalController.BlinkShapes();
        yield return new WaitForSeconds(0.48f);
        experimentalController.BreakConnectionAndPopText();
    }
}
