using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRing : MonoBehaviour
{
    [SerializeField] private PerRendererOpacity crystalOpacity;
    [SerializeField] private PerRendererColor ringColor;
    public void UpdateRingColor(float opacity)
    {
        crystalOpacity.opacity = opacity;
        ringColor.tint.a = opacity;
    }
}
