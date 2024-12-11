using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleExplodeController : MonoBehaviour
{
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private CircleExpandingController expandController;
    [SerializeField] private float explodeRadius;
    [SerializeField] private ParticleSystem p_explode;

    private bool exploded = false;

    // Update is called once per frame
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
        float initRadius = expandCircle.circleRadius;
        float initNoiseMin = expandCircle.noiseMin;
        float initNoise = expandCircle.noiseStrength;
        yield return new WaitForLoop(duration, (t)=>{
            float _t = EasingFunc.Easing.CircEaseOut(t);
            expandCircle.circleRadius = Mathf.Lerp(initRadius, 0.3f, _t);
            expandCircle.noiseMin = Mathf.Lerp(initNoiseMin, 0f, _t);
            expandCircle.noiseStrength = Mathf.Lerp(initNoise, 0.3f, _t);
        }); 
    }
}
