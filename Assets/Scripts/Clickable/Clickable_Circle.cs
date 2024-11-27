using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_Circle : Basic_Clickable
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
    [SerializeField] private float maxfollowSpeed = 10;
    [SerializeField] private float lerpSpeed = 5;
    [SerializeField] private float followFactor = 1;
    [SerializeField, Range(0, 1)] private float speedDrag = 0;
[Header("Circle Animation Control")]
    [SerializeField] private CircleMotion[] circleMotions;
    private float camDepth;
    private Vector3 velocity;

    void Start(){
        camDepth = Camera.main.WorldToScreenPoint(transform.position).z;
    }
    void Update(){
        velocity *= (1-speedDrag);
        for(int i=0; i<circleMotions.Length; i++){
            circleMotions[i].MotionUpdate(velocity);
        }
    }
    void FixedUpdate(){
        transform.position += velocity * Time.fixedDeltaTime;
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        player.HoldInteractable(this);
    }
    public override void ControllingUpdate(PlayerController player)
    {
        Vector3 cursorPoint = player.GetCursorWorldPoint(camDepth);
        Vector3 diff = cursorPoint - transform.position;
        diff = Vector3.ClampMagnitude(diff*followFactor, maxfollowSpeed);

        velocity = Vector3.Lerp(velocity, diff, lerpSpeed*Time.deltaTime);
    }
}
