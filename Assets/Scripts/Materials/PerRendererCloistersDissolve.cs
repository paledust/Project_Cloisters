using UnityEngine;

public class PerRendererCloistersDissolve : PerRendererBehavior
{
    [SerializeField, ColorUsage(true, true)] private Color fillColor = Color.white;

[Header("Material Control")]
    public float dissolveRadius = 0;
    [SerializeField] private bool useRectUV = false;
    [SerializeField] public float emissionScale = 1;
    [SerializeField] private Vector2 dissolveCenter = new Vector2(0.5f, 0.5f);
    
    public bool m_isListenableActivated{get{return gameObject.activeInHierarchy;}}

    private const string FillColorName = "_FillColor";
    private const string SpriteRectName = "_SpriteRect";
    private const string DissolveRadiusName = "_DissolveRadius";
    private const string EmissionScaleName = "_EmissionScale";
    private const string UseRectUVName = "_UseRectUV";
    private const string RadiusCenterXName = "_RadiusCenterX";
    private const string RadiusCenterYName = "_RadiusCenterY";

    protected override void InitProperties()
    {
        base.InitProperties();

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
        mpb.SetColor(FillColorName, fillColor);
    }
}