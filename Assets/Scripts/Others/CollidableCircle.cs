using System.Collections;
using System.Collections.Generic;
using TMPro;
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
[Header("SpawnCircle")]
    [SerializeField, ShowOnly] private bool hasCollided = false;
[Header("Text")]
    [SerializeField] private TextMeshPro txt;
    [SerializeField] private ParticleSystem p_hasText;

    private bool isGrowing = false;
    private bool isSpawning = false;
    private IC_Narrative narrativeController;
    private Clickable_Circle circle;
    private Vector3 targetPoint;

    public bool Collidable => m_collider.enabled;
    public bool CanGrow => !circle.IsGrownCircle && !isGrowing && !isSpawning;
    public bool m_hasCollided => hasCollided;
    public float radius => m_collider.radius * transform.localScale.x;
    public Clickable_Circle m_circle => circle;
    public Rigidbody m_rigidbody => m_rigid;

    private const string GROW_TIER_TWO = "CircleGrow_Class_2";
    private const string GROW_TIER_THREE = "CircleGrow_Class_3";
    private const string FLOAT_ANIMATION = "CircleFloat";
    private const string POPUP_ANIMATION = "CirclePopUp";
    void Awake()
    {
        circle = GetComponent<Clickable_Circle>();
    }
    void Start(){
        if(circle.m_circleType == Clickable_Circle.CircleType.Target)
            p_hasText.Play();
    }
    void Update(){
        circleMotionControl.UpdateCircleMotion(m_rigid.velocity);
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPoint, 0.01f);
        DebugExtension.DrawArrow(m_rigid.position, m_rigid.velocity, Color.blue);
    }
    public void OnCollideWithControlledCircle(Clickable_Circle controlledCircle, Vector3 contact, float strength)
    {
        //产生小球
        hasCollided = true;
        m_rigid.velocity = (m_rigid.position - contact).normalized * strength;
        m_rigid.drag = 6;
        circle.TriggerCollideRipple();

        //固定自身
        StartCoroutine(coroutineCreateJointAtCurrentPos(1));
        if(circle.m_circleType == Clickable_Circle.CircleType.Target)
            p_hasText.Stop();
    }

    #region Circle Spawning
    public void OnCircleSpawned(IC_Narrative narrativeController)
    {
        this.narrativeController = narrativeController;
    }
    #endregion

    #region Circle Motion
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

        isSpawning = false;
        isGrowing  = false;
        circle.ResetWobble();
        int currentClass = circle.m_circleClass;
        for(int i=0; i<resetCircles.Length; i++){
            resetCircles[i].ResetSize(currentClass);
        }
    }
    public void FloatUp(float duration){
        isSpawning = true;
        StartCoroutine(coroutineFloatingUp(duration));
    }
    public void PopUp(float duration)
    {
        isSpawning = true;
        circleAnime[POPUP_ANIMATION].speed = circleAnime[POPUP_ANIMATION].length/duration;
        circleAnime.Play(POPUP_ANIMATION);
    }
    #endregion

    #region Animation Event
    public void AE_EnableHitbox(){
        float size = renderRoot.transform.localScale.x;
        StartCoroutine(coroutineGrowHitbox(2f, size));
    }
    public void AE_FloatDone(){
        isSpawning = false;
    }
    public void AE_GrowingDone(){
        isGrowing = false;
        int circleClass = circle.IncreaseCircleClass();

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
    }
    #endregion

    #region Text
    public void ShowText()
    {
        txt.text = narrativeController.GetNextNarrativeChar().ToString();
        txt.gameObject.SetActive(true);
    }
    #endregion

    IEnumerator coroutineGrowHitbox(float duration, float scaleFactor){
        m_collider.radius = 0;
        circle.EnableHitbox();
        yield return new WaitForLoop(duration, (t)=>{
            m_collider.radius = Mathf.Lerp(0, 2.4f*scaleFactor, t);
        });
    }
    IEnumerator coroutineFloatingUp(float duration){
        Vector3 circlePos = Vector3.zero;
        Vector2 seed = Random.insideUnitCircle;
        circleAnime[FLOAT_ANIMATION].speed = circleAnime[FLOAT_ANIMATION].length/duration;
        circleAnime.Play(FLOAT_ANIMATION);
        yield return new WaitForLoop(duration, (t)=>{
            circlePos.x = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, seed.x)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circlePos.y = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, 0.12345f+seed.y)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circleRoot.localPosition = circlePos;
        });
    }
    IEnumerator coroutineCreateJointAtCurrentPos(float delay){
        yield return new WaitForSeconds(delay);
        PhysicDragManager.PinRigidToCurrentPos(m_rigid, 50, 2);
    }
}
