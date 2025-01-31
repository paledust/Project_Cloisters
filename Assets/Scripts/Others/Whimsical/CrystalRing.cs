using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrystalRing : MonoBehaviour
{
    [SerializeField] private PerRendererOpacity crystalOpacity;
    [SerializeField] private PerRendererColor ringColor;
[Header("Crystal Ring")]
    [SerializeField] private ParticleSystem p_crystal;
    [SerializeField] private float rotationSpeed = 20;

    public void UpdateRingColor(float opacity)
    {
        crystalOpacity.opacity = opacity;
        ringColor.tint.a = opacity;
    }
    public void UpdateRingPhase(float phase)
    {
        p_crystal.transform.Rotate(0,0,phase,Space.Self);
    }
    void Update()
    {
        p_crystal.transform.Rotate(0,0,rotationSpeed*Time.deltaTime,Space.Self);
    }
    public void RefreshParticle()
    {
        p_crystal.Clear();
        p_crystal.Play();
    }
}
