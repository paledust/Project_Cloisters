using UnityEngine;

public class BouncerParticle : MonoBehaviour
{
    [SerializeField] private ParticleSystem p_hit;
    private Bouncer bouncer;

    void Awake()
    {
        bouncer = GetComponent<Bouncer>();
        bouncer.onBounce += HandleBounce;
    }
    void OnDestroy()
    {
        bouncer.onBounce -= HandleBounce;
    }
    void HandleBounce(BounceBall bounceBall)
    {
        p_hit.transform.position = bounceBall.transform.position;
        p_hit.Play();
    }
}
