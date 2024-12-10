using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleMotionControl : MonoBehaviour
{
    [System.Serializable]
    public struct CircleMotion{
        public Transform circleTrans;
        public float lerpSpeed;
        public float controlFactor;
        public float maxOffset;
        public void MotionUpdate(Vector3 motion){
            circleTrans.localPosition = Vector3.Lerp(circleTrans.localPosition, Vector3.ClampMagnitude(motion*controlFactor, maxOffset), Time.deltaTime*lerpSpeed);
        }
    }
    [SerializeField] private CircleMotion[] circleMotions;

    public void UpdateCircleMotion(Vector3 velocity){
        for(int i=0; i<circleMotions.Length; i++){
            circleMotions[i].MotionUpdate(velocity);
        }
    }
}
