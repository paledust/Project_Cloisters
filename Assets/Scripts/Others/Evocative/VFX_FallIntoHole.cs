using UnityEngine;

public class VFX_FallIntoHole : MonoBehaviour
{
    [SerializeField] private Animation m_lightAnime;
    [SerializeField] private ParticleSystem p_light;
    public void PlayFallVFXAt(Vector3 position)
    {
        m_lightAnime.Play();
        p_light.transform.position = position;
        p_light.Play();
    }
}
