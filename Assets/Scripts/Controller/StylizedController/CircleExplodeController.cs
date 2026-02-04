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
    [SerializeField] private string ambKey;
    [SerializeField] private Vector2 ambFade = Vector2.one;

    private bool exploded = false;

    public void ResetController()=>exploded = false;
    void Start()
    {
        AudioManager.Instance.PlayAmbience(ambKey, true, 0.2f, false);
    }
    void Update()
    {
        if(!exploded)
        {
            AudioManager.Instance.SetAmbienceVolume(Mathf.Lerp(ambFade.x, ambFade.y, EasingFunc.Easing.QuadEaseIn(expandCircle.circleRadius/explodeRadius)));
        }
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
        stylizedController.ExplodeToDissolveTransition();
        yield return new WaitForSeconds(0.2f);
        AudioManager.Instance.FadeAmbience(0.026f, 2f);

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