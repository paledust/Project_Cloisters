using System.Collections;
using DG.Tweening;
using UnityEngine;

public class Clickable_Circle : Basic_Clickable
{
    public enum CircleType
    {
        Controlled, //Player Controlled Circle
        Normal, //Normal Circle, spawn new Circles once collide.
        Target, //Target Circle mostly same as Normal Circle, but the newly spanwed circles must contain a narrative circle.
        Narrative, //Contain text, collide will spawn nothing.
    }
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
    [SerializeField] private int circleClass = 3;
    [SerializeField] private float maxfollowSpeed = 10;
    [SerializeField] private float lerpSpeed = 5;
    [SerializeField] private float followFactor = 1;
    [SerializeField, Range(0, 1)] private float speedDrag = 0;
[Header("Particles")]
    [SerializeField] private ParticleSystem p_trail;
    [SerializeField] private ParticleSystem p_ripple;
    [SerializeField] private ParticleSystem p_caustic;
[Header("Collision")]
    [SerializeField] private CircleType circleType;
    [SerializeField] private float bounceFactor;
    [SerializeField] private float collisionFactor;
    [SerializeField] private float boucneScale = 0.01f;
    [SerializeField] private Vector2 bounceRange;
    [SerializeField] private float bounceDuration; 
    [SerializeField] private AnimationCurve bounceCurve;
[Header("Circle Animation Control")]
    [SerializeField] private CircleWobble[] circleWobbles;
    [SerializeField] private CircleMotionControl circleMotionControl;
[Header("Circle Type VFX")]
    [SerializeField] private SpriteRenderer spriteGlow;
[Header("Circle Sprites")]
    [SerializeField] private IC_Narrative.HeroCircleTransistor[] circleTransistors;

    private float camDepth;
    private Rigidbody rigid;
    private SphereCollider sphereCollider;
    private Vector3 velocity;

    public bool IsGrownCircle => circleClass == 3;
    public int m_circleClass => circleClass;
    public float radius => sphereCollider.radius * transform.localScale.x;
    public Rigidbody m_rigid => rigid;
    public CircleType m_circleType => circleType;

    void Awake(){
        rigid = GetComponent<Rigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        camDepth = Camera.main.WorldToScreenPoint(transform.position).z;
    }
    void Update(){
        velocity *= 1-speedDrag;
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
    public void ChangeCircleType(CircleType newType){
        circleType = newType;
        if(circleType == CircleType.Target)
        {
            spriteGlow.DOFade(0.25f, 1f).SetEase(Ease.OutQuad);
        }
    }
    void OnCollisionEnter(Collision collision){
        float strength = collision.relativeVelocity.magnitude;
        float factor = m_rigid.isKinematic?bounceFactor:collisionFactor;
        for(int i=0; i<circleWobbles.Length; i++){
            circleWobbles[i].WobbleCircle(Mathf.Clamp(strength * factor * boucneScale, bounceRange.x, bounceRange.y), bounceCurve, bounceDuration);
        }

        if(!this.isControlling)
            return;
        var otherCircle = collision.gameObject.GetComponent<CollidableCircle>();
        if(otherCircle != null)
        {
            EventHandler.Call_OnClickableCircleCollide(otherCircle.m_circle, this, collision);
        }
    }
    public void OnHeavyCollide()=>p_caustic.Play(true);
    public void TransitionCircles(float duration)
    {
        for(int i=0; i<circleTransistors.Length; i++)
        {
            StartCoroutine(coroutineTransitionCircle(circleTransistors[i], duration));
        }
    }
    IEnumerator coroutineTransitionCircle(IC_Narrative.HeroCircleTransistor circleTransistor, float duration){
        Color initColor = circleTransistor.heroCircleSprite.color;
        yield return new WaitForLoop(duration, (t)=>{
            circleTransistor.heroCircleSprite.color = Color.Lerp(initColor, circleTransistor.transitionColor, EasingFunc.Easing.QuadEaseOut(t));
        });
    }
}
