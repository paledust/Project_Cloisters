using UnityEngine;

public class PerRendererExpShape : PerRendererBehavior
{
    public float emissive = 2;
    protected static readonly int EMISSIVE_ID = Shader.PropertyToID("_Emissive");
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(EMISSIVE_ID, emissive);
    }
}
