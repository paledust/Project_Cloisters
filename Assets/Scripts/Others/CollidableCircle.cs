using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollidableCircle : MonoBehaviour
{
    [System.Serializable]
    public struct ResizableTrans{
        public Transform circleTrans;
        public float classOneSize;
        public float classTwoSize;
        public void ResetSize(int circleClass){
            switch(circleClass){
                case 1:
                    circleTrans.localScale = Vector3.one*classOneSize;
                    break;
                case 2:
                    circleTrans.localScale = Vector3.one*classTwoSize;
                    break;
            }
        }
    }
    [SerializeField] private CircleMotionControl circleMotionControl;
    [SerializeField] private SpriteRenderer m_bigCircleRenderer;
    [SerializeField] private SphereCollider m_collider;
    [SerializeField] private Transform renderRoot;
    [SerializeField] private Clickable_Circle m_circle;
    [SerializeField] private Rigidbody m_rigid;
    [SerializeField] private Animation circleAnime;
[Header("Float")]
    [SerializeField] private Transform circleRoot;
    [SerializeField] private float noiseFreq = 2f;
    [SerializeField] private float noiseAmp = 0.1f;
    [SerializeField] private PerRendererOpacity[] circleOpacity;
[Header("Grow")]
    [SerializeField] private float controlGrowCollisionStrength = 2;
    [SerializeField] private float otherGrowCollisionStrength = 0.01f;
[Header("Reset Size")]
    [SerializeField] private ResizableTrans[] resetCircles;

    private bool isGrowing = false;
    private bool isFloating = false;

    public bool Collidable{get{return m_collider.enabled;}}
    public bool CanGrow{get{return !m_circle.IsGrownCircle && !isGrowing && !isFloating;}}
    public bool IsVisible{get{return m_bigCircleRenderer.isVisible;}}

    private CoroutineExcuter velChanger;
    private Vector3 targetPoint;
    private const string GrowClassTwoClip = "CircleGrow_Class_2";
    private const string GrowClassThreeClip = "CircleGrow_Class_3";
    private const string CircleFloat = "CircleFloat";

    void Start(){
        velChanger = new CoroutineExcuter(this);
    }
    void Update(){
        circleMotionControl.UpdateCircleMotion(m_rigid.velocity);
    }
    void OnCollisionEnter(Collision other){
        velChanger.Abort();

        var otherCircle = other.gameObject.GetComponent<Clickable_Circle>();

        if(!otherCircle.enabled){
            Vector3 separate = other.relativeVelocity;
            separate.z = 0;
            m_rigid.velocity = separate*(0.6f+m_circle.m_circleClass*0.1f);
            other.rigidbody.velocity = -separate*(0.6f+m_circle.m_circleClass*0.1f);
        }

        if(otherCircle.IsGrownCircle){
            float strength = otherCircle.enabled?other.rigidbody.velocity.magnitude:other.relativeVelocity.magnitude;
            if(strength<(otherCircle.enabled?controlGrowCollisionStrength:otherGrowCollisionStrength)) {
                StartCoroutine(CommonCoroutine.delayAction(()=>BeginVelocitySlerp(), 0.25f));
                return;
            }
            if(CanGrow){
                isGrowing = true;
                switch(m_circle.m_circleClass){
                    case 1:
                        circleAnime.Play(GrowClassTwoClip);
                        break;
                    case 2:
                        circleAnime.Play(GrowClassThreeClip);
                        break;
                }
            }
            else{
                StartCoroutine(CommonCoroutine.delayAction(()=>BeginVelocitySlerp(), 0.25f));
            }
        }
        else{
            StartCoroutine(CommonCoroutine.delayAction(()=>BeginVelocitySlerp(), 0.25f));
        }
    }
    public void ResetSize(float size){
        renderRoot.transform.localScale = Vector3.one * size;
        m_collider.radius = 0.16f*size;
    }
    public void ResetMotion(){
        m_rigid.velocity = Vector3.zero;
    }
    public void ResetGrowingAndWobble(){
        for(int i=0; i<circleOpacity.Length; i++)
            circleOpacity[i].opacity = 0;

        isFloating = false;
        isGrowing  = false;
        m_circle.ResetWobble();
        int currentClass = m_circle.m_circleClass;
        for(int i=0; i<resetCircles.Length; i++){
            resetCircles[i].ResetSize(currentClass);
        }
    }
    public void FloatUp(float duration){
        isFloating = true;
        StartCoroutine(coroutineFloatingUp(duration));
    }
    public void AE_EnableHitbox(){
        float size = renderRoot.transform.localScale.x;
        StartCoroutine(coroutineGrowHitbox(2f, size));
    }
    public void AE_FloatDone(){
        isFloating = false;
    }
    public void AE_GrowingDone(){
        isGrowing = false;
        int circleClass = m_circle.IncreaseCircleClass();

        switch(circleClass){
            case 2:
                m_rigid.mass = 3;
                m_rigid.drag = 3.5f;
                break;
            case 3:
                m_rigid.mass = 8;
                m_rigid.drag = 5;
                // m_circle.enabled = true;
                // m_circle.EnableRaycast();

                break;
        }
        BeginVelocitySlerp();
    }
    void BeginVelocitySlerp(){
        Vector3 point = SmallCircleSpawner.m_rectSelector.GetPoint();
        targetPoint = point;
        point.z = transform.position.z;
        velChanger.Excute(coroutineSlerpVelocity(point, 0.2f, Random.Range(0.05f, 0.08f), Random.Range(4f, 5f)));
    }
    IEnumerator coroutineSlerpVelocity(Vector3 target, float velFactor, float maxMag, float duration){
        Vector3 startVel = m_rigid.velocity;
        Vector3 targetVel = Vector3.ClampMagnitude(velFactor*(target - m_rigid.position), maxMag);
        yield return new WaitForLoop(duration, (t)=>{
            m_rigid.velocity = Vector3.Slerp(startVel, targetVel, t);
        });
        BeginVelocitySlerp();
    }
    IEnumerator coroutineGrowHitbox(float duration, float scaleFactor){
        m_collider.radius = 0;
        m_circle.EnableHitbox();
        yield return new WaitForLoop(duration, (t)=>{
            m_collider.radius = Mathf.Lerp(0, 2.4f*scaleFactor, t);
        });
    }
    IEnumerator coroutineFloatingUp(float duration){
        Vector3 circlePos = Vector3.zero;
        Vector2 seed = Random.insideUnitCircle;
        circleAnime[CircleFloat].speed = circleAnime[CircleFloat].length/duration;
        circleAnime.Play(CircleFloat);
        yield return new WaitForLoop(duration, (t)=>{
            circlePos.x = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, seed.x)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circlePos.y = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, 0.12345f+seed.y)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circleRoot.localPosition = circlePos;
        });

        BeginVelocitySlerp();
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPoint, 0.01f);
        DebugExtension.DrawArrow(m_rigid.position, m_rigid.velocity, Color.blue);
    }
}
