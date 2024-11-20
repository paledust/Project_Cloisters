using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MotionSetController : MonoBehaviour
{
    [SerializeField] private Clickable_Planet clickable_Planet;
    [SerializeField] private float speedScale;
    [SerializeField] private float speedOffset;
    [SerializeField] private float controlAgility = 5;
[Header("Wave Control")]
    [SerializeField] private PerRendererWave[] waves;
    [SerializeField] private float waveFactor = 1;
[Header("Sky control")]
    [SerializeField] private Transform skyTrans;
    [SerializeField] private float maxOffset = -33f;
    [SerializeField] private float skySpeedFactor = 1f;
[Header("Circles")]
    [SerializeField] private FloatingMotion[] floatingMotions;
    [SerializeField] private float motionFactor = 1;

    private float speed = 0;

    void Update()
    {
        speed = Mathf.Lerp(speed, clickable_Planet.m_angularSpeed*speedScale - (clickable_Planet.m_isControlling?0:speedOffset), Time.deltaTime * controlAgility);

        Vector3 pos = skyTrans.position;
        pos += Vector3.right * speed * skySpeedFactor * Time.deltaTime;
        pos.x = Mathf.Clamp(pos.x, -maxOffset, maxOffset);
        skyTrans.position = pos;

        for(int i=0; i<waves.Length; i++){
            waves[i].wavePhase += speed * waveFactor * Time.deltaTime;
        }

        for(int i=0; i<floatingMotions.Length; i++){
            floatingMotions[i].floatPhase += speed * motionFactor * Time.deltaTime;
        }
    }
}
