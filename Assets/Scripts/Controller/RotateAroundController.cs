using UnityEngine;

public class RotateAroundController : MonoBehaviour
{
    [SerializeField] private RotateAround rotateAround;
    [SerializeField] private Clickable_ObjectRotator clickable_Planet;
    [SerializeField] private float controlAgility = 5;
[Header("Speed Remapping")]
    [SerializeField] private float speedScale;
    [SerializeField] private float maxSpeed;

[Header("Sphere")]
    [SerializeField] private Animation blueSphere;
    [SerializeField] private Transform pivotSphere;
    [SerializeField] private float fadeInAngle = 30;

    private float targetSpeed;
    private bool sphereFadeIn;

    void Start()
    {
        targetSpeed = 0;
    }
    void Update()
    {
        targetSpeed = Mathf.Lerp(targetSpeed, clickable_Planet.m_angularSpeed*speedScale, Time.deltaTime * controlAgility);
        targetSpeed = Mathf.Clamp(targetSpeed, -maxSpeed, maxSpeed);
        rotateAround.angularSpeed = targetSpeed;

        Vector3 dir = blueSphere.transform.position - pivotSphere.position;
        float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

        if(!sphereFadeIn && Mathf.Abs(angle) >= fadeInAngle)
        {
            sphereFadeIn = true;
            blueSphere.Play();
        }
    }
}
