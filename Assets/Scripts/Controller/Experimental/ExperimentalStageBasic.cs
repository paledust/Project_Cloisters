using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ExperimentalStageBasic : MonoBehaviour
{
    [SerializeField] protected IC_Experimental experimentalController;
    public virtual void CompleteStage(){}
    public abstract bool IsDone();
}
