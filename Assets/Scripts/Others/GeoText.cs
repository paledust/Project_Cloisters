using System.Collections;
using DG.Tweening;
using UnityEngine;

public class GeoText : MonoBehaviour
{
    [SerializeField] private FloatingMotion floatingMotion;
    [SerializeField] private AngleMotion angleMotion;
    public void StopMotion(float duration)=>StartCoroutine(coroutineStopMotion(duration));
    public void PunchRender(float strength, float duration, int vibration)
    {
        floatingMotion.transform.DOPunchScale(Vector3.one * strength, duration, vibration);
    }
    IEnumerator coroutineStopMotion(float duration){
        yield return new WaitForLoop(duration, (t)=>{
            floatingMotion.ControlFactor = EasingFunc.Easing.SmoothInOut(1-t);
            angleMotion.ControlFactor = EasingFunc.Easing.SmoothInOut(1-t);
        });
        floatingMotion.enabled = false;
        angleMotion.enabled = false;
    }
}
