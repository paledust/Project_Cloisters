using UnityEngine;

public class PerRendererEmission : PerRendererBehavior
{
    [Range(0, 10f)] public float emission = 0;
    protected static readonly int EMISSION_ID = Shader.PropertyToID("_Emission");
    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(EMISSION_ID, emission);
    }
}
