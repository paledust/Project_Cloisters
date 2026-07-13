using UnityEngine;

public class Goal_Crack : MonoBehaviour
{
    [SerializeField] private ParticleSystem p_crack_mask;

    void OnEnable()
    {
        EventHandler.E_OnBallSuperCharge += DisableCrackParticle;
    }
    void OnDisable()
    {
        EventHandler.E_OnBallSuperCharge -= DisableCrackParticle;
    }
    void DisableCrackParticle()
    {
        Destroy(this);        
    }
    void OnCollisionEnter(Collision collision)
    {
        Vector3 pos = collision.contacts[0].point;
        pos.z = transform.position.z;
        p_crack_mask.transform.position = pos;
        p_crack_mask.Play();
    }
}