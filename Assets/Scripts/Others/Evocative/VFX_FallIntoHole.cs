using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFX_FallIntoHole : MonoBehaviour
{
    [SerializeField] private Animation m_lightAnime;
    [SerializeField] private ParticleSystem p_light;
    public void PlayFallVFX()
    {
        m_lightAnime.Play();
        p_light.Play();
    }
}
