using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererCloseLine : PerRendererBehavior
{
    [SerializeField] private Color color;
    [SerializeField, Range(0, 1)] private float progress;
    [SerializeField] private float tiling = 50;

    private static readonly int PROGRESS_ID = Shader.PropertyToID("_Progress");
    private static readonly int TILING_ID = Shader.PropertyToID("_Tiling");

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetColor(COLOR_ID, color);
        mpb.SetFloat(TILING_ID, tiling);
        mpb.SetFloat(PROGRESS_ID, progress);
    }
}
