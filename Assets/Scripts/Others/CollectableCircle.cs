using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableCircle : MonoBehaviour
{
    [SerializeField] private PerRendererDissolve[] circleRender;
    void OnTriggerEnter(Collider other){
        if(other.gameObject.layer == Service.InteractableLayer){
            GetComponent<Collider>().enabled = false;
            StartCoroutine(coroutineGoTo(other.transform, 1.7f, 1f));
            foreach(var perRendererDissolve in circleRender){
                StartCoroutine(coroutineFadeCircle(perRendererDissolve, 0.8f, 0.2f));
            }
        }
    }
    IEnumerator coroutineFadeCircle(PerRendererDissolve targetRenderer, float duraiton, float delay){
        yield return new WaitForSeconds(delay);
        float initDissolve = targetRenderer.DissolveStart;
        yield return new WaitForLoop(duraiton, (t)=>{
            targetRenderer.DissolveStart = Mathf.Lerp(initDissolve, -0.5f, EasingFunc.Easing.QuadEaseOut(t));
        });
        targetRenderer.enabled = false;
    }
    IEnumerator coroutineGoTo(Transform targetTrans, float targetSize, float duration){
        Vector3 initPos = transform.position;
        Vector3 initSize = transform.localScale;
        yield return new WaitForLoop(duration, (t)=>{
            transform.position = Vector3.LerpUnclamped(initPos, targetTrans.position, EasingFunc.Easing.CircEaseOut(t));
            transform.localScale = Vector3.LerpUnclamped(initSize, targetSize*Vector3.one, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
}
