using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DanceText : MonoBehaviour
{
    [SerializeField] private Vector2 danceStepRange;
    [SerializeField] private float danceRadius;
    [SerializeField] private float danceAngle;

    private float danceTimer;
    private float danceStep;
    private Vector3 initPos;

    void Start()
    {
        initPos = transform.localPosition;
        danceStep = Random.Range(danceStepRange.x, danceStepRange.y);
        danceStep *= Random.value;
    }
    void Update()
    {
        danceTimer += Time.deltaTime;
        if (danceTimer >= danceStep)
        {
            danceTimer = 0;
            danceStep = Random.Range(danceStepRange.x, danceStepRange.y);
            transform.localPosition = initPos + (Vector3)Random.insideUnitCircle * danceRadius;
            transform.localRotation = Quaternion.Euler(0,0,Random.Range(-danceAngle, danceAngle));
        }
    }

}
