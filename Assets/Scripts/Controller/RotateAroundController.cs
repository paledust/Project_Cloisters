using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class RotateAroundController : MonoBehaviour
{
    [SerializeField] private RotateAround rotateAround;
    [SerializeField] private Clickable_ObjectRotator clickable_Planet;
    [SerializeField] private float controlAgility = 5;
[Header("Speed Remapping")]
    [SerializeField] private float speedScale;

[Header("Sphere")]
    [SerializeField] private Animation blueSphere;
    [SerializeField] private Transform pivotSphere;
    [SerializeField] private float fadeInAngle = 30;

[Header("Audio")]
    [SerializeField] private string sfx_shine;
    [SerializeField] private float volume = 0.1f;
    private bool sphereFadeIn;

    void Update()
    {
        rotateAround.angularSpeed = Mathf.Lerp(rotateAround.angularSpeed, clickable_Planet.m_angularSpeed*speedScale, Time.deltaTime * controlAgility);

        Vector3 euler = rotateAround.m_target.eulerAngles;
        if(euler.x > 180) euler.x -= 360;
        euler *= 0.5f;

        Vector3 dir = blueSphere.transform.position - pivotSphere.position;
        float angle = Vector3.SignedAngle(Vector3.forward, dir, Vector3.up);

        if(!sphereFadeIn && Mathf.Abs(angle) >= fadeInAngle)
        {
            sphereFadeIn = true;
            blueSphere.Play();
            AudioManager.Instance.PlaySoundEffect(sfx_shine, volume);
        }
    }
}
