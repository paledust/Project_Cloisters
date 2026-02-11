using System.Collections;
using DG.Tweening;
using SimpleAudioSystem;
using UnityEngine;

public class CircleExplodeController : MonoBehaviour
{
    [SerializeField] private IC_Stylized stylizedController;
    [SerializeField] private PerRendererExpand expandCircle;
    [SerializeField] private Clickable_ObjectRotator clickable_Planet;
    [SerializeField] private CircleExpandingController expandController;
    [SerializeField] private Hoverable_DrumInteraction hoverable_drum;
    [SerializeField] private float explodeRadius;
    [SerializeField] private ParticleSystem p_explode;
    [SerializeField] private float targetRadius = 0.3f;
    [SerializeField] private float targetNoise = 0.3f;

    [Header("Audio")]
    [SerializeField] private string explodeSFX;

    private bool exploded = false;

    public void ResetController()=>exploded = false;
    void Update()
    {
        if(expandCircle.circleRadius>=explodeRadius && !exploded)
        {
            Explode();
        }
    }
    public void Explode()
    {
        exploded = true;
        expandController.enabled = false;
        StartCoroutine(coroutineExplode(1f));
        StylizedDrumController.Instance.QueueBeat(explodeSFX, 1);
        clickable_Planet.transform.DOScale(Vector3.one * 0.375f, 0.1f).SetEase(Ease.OutQuad).OnComplete(() =>
        {
            clickable_Planet.transform.DOScale(Vector3.one*0.3f, 0.3f).SetEase(Ease.InOutQuad);
        });
    }

    IEnumerator coroutineExplode(float duration){
        p_explode.Play(true);
        clickable_Planet.BreakSpring();
        clickable_Planet.enabled = false;
        clickable_Planet.DisableClicking();
        hoverable_drum.enabled = true;
        EventHandler.Call_OnFlushInput();
        stylizedController.StylizedExplode();

        float initRadius = expandCircle.circleRadius;
        float initNoiseMin = expandCircle.noiseMin;
        float initNoise = expandCircle.noiseStrength;
        yield return new WaitForLoop(duration, (t)=>{
            float _t = EasingFunc.Easing.CircEaseOut(t);
            expandCircle.circleRadius = Mathf.Lerp(initRadius, targetRadius, _t);
            expandCircle.noiseMin = Mathf.Lerp(initNoiseMin, 0f, _t);
            expandCircle.noiseStrength = Mathf.Lerp(initNoise, targetNoise, _t);
        });
        stylizedController.StartExtending();
    }
}