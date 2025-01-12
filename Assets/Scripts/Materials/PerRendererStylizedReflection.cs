using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerRendererStylizedReflection : PerRendererBehavior
{
    [SerializeField] private float reflectionStrength = 0.5f;

    private const string ReflectionStrengthName = "_ReflectionStrength";

    protected override void UpdateProperties()
    {
        base.UpdateProperties();
        mpb.SetFloat(ReflectionStrengthName, reflectionStrength);
    }
}
