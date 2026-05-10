using System;
using System.Collections;
using SimpleAudioSystem;
using UnityEngine;

public class CollidableCircle : MonoBehaviour
{
    [Serializable]
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
[Header("Reset Size")]
    [SerializeField] private ResizableTrans[] resetCircles;
[Header("SpawnCircle")]
    [SerializeField, ShowOnly] private bool hasCollided = false;
[Header("Text")]
    [SerializeField] private NarrativeTxtSprite narrativeTxt;
    [SerializeField] private ParticleSystem p_hasText;

[Header("VFX Explode Resolve")]
    [SerializeField] private Transform vfxRoot;

[Header("Audio")]
    [SerializeField] private AudioSource loopSource;
    [SerializeField] private string sfx_drag_circle;
    [SerializeField] private float maxVolume = .5f;
    [SerializeField] private float speedGate = 0.1f;
    [SerializeField] private float dragToVolumeScale = 1;
    [SerializeField] private float speedLerp = 4f;

    private Clickable_Circle circle;
    private NarrativeCircleNode narrativeCircleNode;
    private Vector3 targetPoint;

    public bool m_hasCollided => hasCollided;
    public bool isPined{get; private set;} = false;
    public Clickable_Circle m_circle => circle;
    public Rigidbody m_rigidbody => m_rigid;

    private const string EXPLODE_ANIMATION = "CircleExplode";
    private const string FLOAT_ANIMATION = "CircleFloat";
    private const string POPUP_ANIMATION = "CirclePopUp";
    private const string HOLLOW_ANIMATION = "CircleHollow";
    private float audioSpeed;
    [SerializeField] private float audioTime;

    private Action onCircleExplode;

    void Awake()
    {
        circle = GetComponent<Clickable_Circle>();
        narrativeCircleNode = GetComponent<NarrativeCircleNode>();
        audioSpeed = 0;
        audioTime = 0;
    }
    void Start(){
        if(circle.m_circleType == Clickable_Circle.CircleType.Target)
            p_hasText.Play();
    }
    void Update(){
        circleMotionControl.UpdateCircleMotion(m_rigid.velocity);

        float speed = m_rigid.velocity.magnitude;
        audioSpeed = Mathf.Lerp(audioSpeed, speed, Time.deltaTime*speedLerp);
        float volume = Mathf.Clamp((audioSpeed-speedGate)*dragToVolumeScale, 0, maxVolume);

        if(loopSource.isPlaying)
            loopSource.volume = volume;

        if(audioSpeed > speedGate && !loopSource.isPlaying)
        {
            AudioManager.Instance.PlaySFXLoop(loopSource, sfx_drag_circle, volume, 0, audioTime);
        }
        else if(audioSpeed <= speedGate && loopSource.isPlaying)
        {
            audioTime = loopSource.time;
            loopSource.Stop();
        }
    }
    void OnDestroy()
    {
        onCircleExplode = null;
    }
    void OnDrawGizmos(){
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPoint, 0.01f);
        DebugExtension.DrawArrow(m_rigid.position, m_rigid.velocity, Color.blue);
    }
    public void OnCollideWithControlledCircle(Clickable_Circle controlledCircle, Vector3 contact, float strength)
    {
        hasCollided = true;
        if(m_circle.m_circleType == Clickable_Circle.CircleType.Narrative)
        {
            m_rigid.velocity = (m_rigid.position - contact).normalized * Mathf.Max(strength, 2f);
            ExplodeCircle();
            return;
        }
        circle.TriggerCollideRipple();

        //固定自身
        if(circle.m_circleType == Clickable_Circle.CircleType.Target)
            p_hasText.Stop();
    }
    public void ExplodeCircle()
    {
        circleAnime.Play(EXPLODE_ANIMATION);
        vfxRoot.SetParent(null);
        narrativeTxt.transform.SetParent(transform);
        Destroy(vfxRoot.gameObject, 4f);
    }
    public void HollowCircle()
    {
        circleAnime.Play(HOLLOW_ANIMATION);
    }

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

        circle.ResetWobble();
        int currentClass = circle.m_circleClass;
        for(int i=0; i<resetCircles.Length; i++){
            resetCircles[i].ResetSize(currentClass);
        }
    }
    public void FloatUp(float duration){
        StartCoroutine(coroutineFloatingUp(duration));
    }
    public void PopUp(float duration)
    {
        circleAnime[POPUP_ANIMATION].speed = circleAnime[POPUP_ANIMATION].length/duration;
        circleAnime.Play(POPUP_ANIMATION);
    }
    #endregion

    #region Animation Event
    public void AE_OnExplode()
    {
        EventHandler.Call_OnNarrativeExplode(this);
        //move text to world and detach from circle
        if(narrativeTxt.gameObject.activeSelf)
        {
            narrativeTxt.transform.SetParent(null);
            narrativeTxt.OnShowingText(m_rigid.velocity);
        }
        narrativeCircleNode.NodeExplode();
        onCircleExplode?.Invoke();
    }
    public void AE_EnableHitbox(){
        float size = renderRoot.transform.localScale.x;
        StartCoroutine(coroutineGrowHitbox(2f, size));
    }
    public void AE_FloatDone(){}
    public void AE_ExplodeDone()
    {
        Destroy(gameObject);
    }
    #endregion

    #region Text
    public void ShowText(Sprite textSprite)
    {
        narrativeTxt.AssignSprite(textSprite);
        narrativeTxt.gameObject.SetActive(true);
    }
    public void RegisterOnExplode(Action onExplode)
    {
        this.onCircleExplode += onExplode;
    }
    public void UnregisterOnExplode(Action onExplode)
    {
        this.onCircleExplode -= onExplode;
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
        Vector2 seed = UnityEngine.Random.insideUnitCircle;
        circleAnime[FLOAT_ANIMATION].speed = circleAnime[FLOAT_ANIMATION].length/duration;
        circleAnime.Play(FLOAT_ANIMATION);
        yield return new WaitForLoop(duration, (t)=>{
            circlePos.x = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, seed.x)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circlePos.y = noiseAmp * (Mathf.PerlinNoise(t*noiseFreq, 0.12345f+seed.y)*2-1) * EasingFunc.Easing.QuadEaseIn(1-t);
            circleRoot.localPosition = circlePos;
        });
    }
}
