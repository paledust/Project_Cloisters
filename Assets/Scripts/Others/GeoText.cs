using System.Collections;
using UnityEngine;

public class GeoText : MonoBehaviour
{
    [SerializeField] private FloatingMotion floatingMotion;
    [SerializeField] private AngleMotion angleMotion;
    public void StopMotion(float duration)=>StartCoroutine(coroutineStopMotion(duration));
    IEnumerator coroutineStopMotion(float duration){
        yield return new WaitForLoop(duration, (t)=>{
            floatingMotion.ControlFactor = EasingFunc.Easing.SmoothInOut(1-t);
            angleMotion.ControlFactor = EasingFunc.Easing.SmoothInOut(1-t);
        });
        floatingMotion.enabled = false;
        angleMotion.enabled = false;
    }
}
