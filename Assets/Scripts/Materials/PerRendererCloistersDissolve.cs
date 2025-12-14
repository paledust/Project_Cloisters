using System;
using System.Collections;
using UnityEngine;

public class PerRendererCloistersDissolve : PerRendererBehavior
{
[Header("Dissolve Control")]
    [SerializeField] public bool autoHideWhenNotSense = true;
    [SerializeField] private float hideRadius = 0;
    [SerializeField] private float fullRadius = 1;
    [SerializeField] private float fadeDuration;
    [SerializeField] private AnimationCurve fadeOverrideCurve;

[Header("Material Control")]
    [SerializeField] private bool useRectUV = false;
    [SerializeField] private float dissolveRadius = 0;
    [SerializeField] public float emissionScale = 1;
    [SerializeField] private Vector2 dissolveCenter = new Vector2(0.5f, 0.5f);
    
    public bool m_isListenableActivated{get{return gameObject.activeInHierarchy;}}

    private bool isRevealed;
    private CoroutineExcuter totemDissolver;

    private const string SpriteRectName = "_SpriteRect";
    private const string DissolveRadiusName = "_DissolveRadius";
    private const string EmissionScaleName = "_EmissionScale";
    private const string UseRectUVName = "_UseRectUV";
    private const string RadiusCenterXName = "_RadiusCenterX";
    private const string RadiusCenterYName = "_RadiusCenterY";

    protected override void InitProperties()
    {
        base.InitProperties();
        totemDissolver = new CoroutineExcuter(this);

        Vector4 rect;
        rect.x = (m_Renderer as SpriteRenderer).sprite.textureRect.xMin;
        rect.y = (m_Renderer as SpriteRenderer).sprite.textureRect.yMin;
        rect.z = (m_Renderer as SpriteRenderer).sprite.textureRect.width;
        rect.w = (m_Renderer as SpriteRenderer).sprite.textureRect.height;
        mpb.SetVector(SpriteRectName, rect);
        mpb.SetFloat(UseRectUVName, useRectUV?1:0);

        UpdateProperties();
    }
    protected override void UpdateProperties()
    {
        mpb.SetFloat(RadiusCenterXName, dissolveCenter.x);
        mpb.SetFloat(RadiusCenterYName, dissolveCenter.y);
        mpb.SetFloat(DissolveRadiusName, dissolveRadius);
        mpb.SetFloat(EmissionScaleName, emissionScale);
    }
    public void RevealTotem(){
        if(!isRevealed) isRevealed = true;
        totemDissolver.Excute(coroutineDissolveInTotem(fadeDuration));
    }
    public void RevealTotem(float dissolveInTime){
        if(!isRevealed) isRevealed = true;
        totemDissolver.Excute(coroutineDissolveInTotem(dissolveInTime));
    }
    public void HideTotem(){
        if(isRevealed) isRevealed = false;
        totemDissolver.Excute(coroutineDissolveTotem(hideRadius, fadeDuration));
    }
    public void HideTotem(float duration){
        if(isRevealed) isRevealed = false;
        totemDissolver.Excute(coroutineDissolveTotem(hideRadius, duration));
    }
    IEnumerator coroutineDissolveInTotem(float duration){
        dissolveRadius = hideRadius;
        yield return coroutineDissolveTotem(fullRadius, duration);
    }
    IEnumerator coroutineDissolveTotem(float targetRadius, float duration){
        bool useCurve = fadeOverrideCurve.keys.Length>=2;
        float initRadius = dissolveRadius;
        yield return new WaitForLoop(duration, (t)=>{
            if(useCurve) dissolveRadius = Mathf.Lerp(initRadius, targetRadius, fadeOverrideCurve.Evaluate(t));
            else dissolveRadius = Mathf.Lerp(initRadius, targetRadius, t);
        });
    }
}