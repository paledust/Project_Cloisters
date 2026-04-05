using UnityEngine;

public class CrystalRotation : MonoBehaviour
{
    [Header("Crystal")]
    [SerializeField] private Transform crystalTrans;
    [SerializeField] private float selfRotateSpeed = 5;
    [SerializeField] private float xPosToRotation = 4;
    [Header("Particle")]
    [SerializeField] private ParticleSystem blinkParticle;
    [SerializeField] private float particleAngleScale = 0.25f;

    private float particleAngle;
    private float angle;

    void Start()
    {
        angle = Random.Range(-50f, 50f);
        particleAngle = (angle + (transform.position.x * xPosToRotation)) * particleAngleScale;   
    }
    void Update()
    {
        angle += selfRotateSpeed * Time.deltaTime;
        float targetAngle = angle + (transform.position.x * xPosToRotation);
        crystalTrans.localRotation = Quaternion.Euler(0, targetAngle, 0);

        particleAngle = targetAngle * particleAngleScale;
        blinkParticle.transform.localRotation = Quaternion.Euler(0, particleAngle, 0);
    }
}