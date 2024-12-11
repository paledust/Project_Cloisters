using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleExplodeController : MonoBehaviour
{
    [SerializeField] private IC_Stylized stylizedController;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private CircleExpandingController expandController;
    [SerializeField] private float explodeRadius;
    [SerializeField] private ParticleSystem p_explode;
    [SerializeField] private float targetRadius = 0.3f;
    [SerializeField] private float targetNoise = 0.3f;
    private bool exploded = false;

    void OnDisable(){
        exploded = false;
    }
    void Update()
    {
        if(expandCircle.circleRadius>=explodeRadius && !exploded){
            exploded = true;
            expandController.enabled = false;
            StartCoroutine(coroutineExplode(1f));
        }
    }
    IEnumerator coroutineExplode(float duration){
        p_explode.Play(true);
        clickable_Planet.BreakSpring();
        EventHandler.Call_OnFlashInput();
        float initRadius = expandCircle.circleRadius;
        float initNoiseMin = expandCircle.noiseMin;
        float initNoise = expandCircle.noiseStrength;
        yield return new WaitForLoop(duration, (t)=>{
            float _t = EasingFunc.Easing.CircEaseOut(t);
            expandCircle.circleRadius = Mathf.Lerp(initRadius, targetRadius, _t);
            expandCircle.noiseMin = Mathf.Lerp(initNoiseMin, 0f, _t);
            expandCircle.noiseStrength = Mathf.Lerp(initNoise, targetNoise, _t);
        });
        stylizedController.StartDissovle();
    }
}
