using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_Circle : Basic_Clickable
{
    [System.Serializable]
    public class CircleWobble{
        public PerRendererWobbles perRendererWobbles;
        [Range(0, 1)]
        public float WobbleFactor;

        private bool isWobbling;
        private CoroutineExcuter wobbler;

        public void WobbleCircle(float wobbleStrength, AnimationCurve wobbleCurve, float duration){
            if(wobbler==null) wobbler = new CoroutineExcuter(perRendererWobbles);

            if(!isWobbling)
                wobbler.Excute(coroutineWobble(wobbleStrength*WobbleFactor, wobbleCurve, duration));
        }
        public void ResetWobble(){
            isWobbling = false;
            perRendererWobbles.WobbleStrength = 0;
        }
        IEnumerator coroutineWobble(float wobbleStrength, AnimationCurve wobbleCurve, float duration){
            isWobbling = true;
            var wobble = perRendererWobbles;
            yield return new WaitForLoop(duration, (t)=>{
                wobble.WobbleStrength = wobbleStrength*wobbleCurve.Evaluate(t);
                if(t>0.75f && isWobbling) isWobbling = false;
            });
            isWobbling = false;
        }
    }
[Header("Circle Control")]
    [SerializeField] private Rigidbody m_rigid;
    [SerializeField] private int circleClass = 3;
    [SerializeField] private float maxfollowSpeed = 10;
    [SerializeField] private float lerpSpeed = 5;
    [SerializeField] private float followFactor = 1;
    [SerializeField, Range(0, 1)] private float speedDrag = 0;
[Header("Particles")]
    [SerializeField] private ParticleSystem p_trail;
    [SerializeField] private ParticleSystem p_ripple;
[Header("Collision")]
    [SerializeField] private float bounceFactor;
    [SerializeField] private float collisionFactor;
    [SerializeField] private float boucneScale = 0.01f;
    [SerializeField] private Vector2 bounceRange;
    [SerializeField] private float bounceDuration; 
    [SerializeField] private AnimationCurve bounceCurve;
[Header("Circle Animation Control")]
    [SerializeField] private CircleWobble[] circleWobbles;
    [SerializeField] private CircleMotionControl circleMotionControl;

    private float camDepth;
    private Vector3 velocity;

    public bool IsGrownCircle{get{return circleClass == 3;}}
    public int m_circleClass{get{return circleClass;}}

    void Start(){
        camDepth = Camera.main.WorldToScreenPoint(transform.position).z;
    }
    void Update(){
        velocity *= (1-speedDrag);
        circleMotionControl.UpdateCircleMotion(velocity);
    }
    void FixedUpdate(){
        if(m_rigid.isKinematic)
            m_rigid.MovePosition(m_rigid.position + velocity * Time.fixedDeltaTime);
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);

        player.HoldInteractable(this);
        p_trail.Play(true);
        m_rigid.isKinematic = true;
        EventHandler.Call_OnControlCircle(this);
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        p_trail.Stop(true);
        m_rigid.isKinematic = false;
    }

    public override void ControllingUpdate(PlayerController player)
    {
        Vector3 cursorPoint = player.GetCursorWorldPoint(camDepth);
        Vector3 diff = cursorPoint - transform.position;
        diff = Vector3.ClampMagnitude(diff*followFactor, maxfollowSpeed);

        velocity = Vector3.Lerp(velocity, diff, lerpSpeed*Time.deltaTime);
    }
    public int IncreaseCircleClass(){
        circleClass ++;
        return circleClass;
    }
    public void ResetWobble(){
        for(int i=0; i<circleWobbles.Length; i++){
            circleWobbles[i].ResetWobble();
        }        
    }
    public void TriggerCollideRipple()=>p_ripple.Play(true);
    void OnCollisionEnter(Collision collision){
        float strength = collision.rigidbody.velocity.magnitude;
        float factor = m_rigid.isKinematic?bounceFactor:collisionFactor;
        for(int i=0; i<circleWobbles.Length; i++){
            circleWobbles[i].WobbleCircle(Mathf.Clamp(strength * factor * boucneScale, bounceRange.x, bounceRange.y), bounceCurve, bounceDuration);
        }

        var otherCircle = collision.gameObject.GetComponent<Clickable_Circle>();
        if(otherCircle.IsGrownCircle && IsGrownCircle){
            EventHandler.Call_OnClickableCircleCollide(otherCircle, collision.contacts[0].point, transform.position - collision.transform.position, strength);
        }
    }
}
