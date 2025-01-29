using System.Collections;
using UnityEngine;

public class Clickable_ObjectRotator : Basic_Clickable
{
    [SerializeField] private Transform pitchRotationTrans;
    [SerializeField] private Transform yawRotationTrans;
[Header("Control")]
    [SerializeField] private float dragStrength = 1f;
    [SerializeField] private float idleAngularSpeed = 2f;
    [SerializeField] private float maxAngularSpeed = 200f;
    [SerializeField] private bool verticalAccumulate = false;
    [SerializeField] private float maxVerticalAngle = 20;
[Header("Resize")]
    [SerializeField] private float resizeFactor = 1.02f;
    [SerializeField] private float resizeTime = 0.4f;
    [SerializeField] private float backTime = 0.5f;
[Header("AngularSpeed Lerp")]
    [SerializeField] private float controllingAngularLerp = 10f;
    [SerializeField] private float releaseAngularLerp = 1f;
[Header("Spring")]
    [SerializeField] private bool useSpring = false;
    [SerializeField] private float springFactor = 2f;

    public float m_angularSpeed{get; private set;}
    public float m_accumulateYaw{get; private set;}
    public bool m_isControlling{get{return playerController!=null;}}

    private float pitchOffset = 0;
    private float targetPitchAngle;
    private float pitchAngle;
    private float zeroSpringAngle = 0;
    
    private PlayerController playerController;
    private CoroutineExcuter sizeChanger;

    void Start(){
        sizeChanger = new CoroutineExcuter(this);
        pitchOffset = pitchRotationTrans.localEulerAngles.x;
        pitchAngle = pitchOffset;
    }
    void Update(){
        if(playerController!=null){
            Vector2 delta = playerController.PointerDelta;
            
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, Mathf.Clamp(delta.x * dragStrength, -maxAngularSpeed, maxAngularSpeed), Time.deltaTime*controllingAngularLerp);

            targetPitchAngle += delta.y * dragStrength * 0.5f * Time.deltaTime;
            targetPitchAngle = Mathf.Clamp(targetPitchAngle, -maxVerticalAngle - pitchOffset, maxVerticalAngle - pitchOffset);
            float finalAngle = targetPitchAngle + pitchOffset;
            pitchAngle = Mathf.Lerp(pitchAngle, finalAngle, Time.deltaTime*controllingAngularLerp);
        }
        else{
            float finalAngle = 0;
            if(verticalAccumulate) 
                finalAngle = targetPitchAngle + pitchOffset;
            else 
                finalAngle = pitchOffset;

            pitchAngle = Mathf.Lerp(pitchAngle, finalAngle, Time.deltaTime*releaseAngularLerp);
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, idleAngularSpeed, Time.deltaTime*releaseAngularLerp);
            if(Mathf.Abs(m_angularSpeed-idleAngularSpeed)<=0.01f) m_angularSpeed = idleAngularSpeed;
        }
    }
    void FixedUpdate(){
        if(useSpring)m_angularSpeed += (m_accumulateYaw-zeroSpringAngle)*springFactor*Time.fixedDeltaTime;
        m_accumulateYaw -= m_angularSpeed*Time.fixedDeltaTime;
        yawRotationTrans.Rotate(Vector3.forward, -m_angularSpeed*Time.fixedDeltaTime, Space.Self);
        pitchRotationTrans.localRotation = Quaternion.Euler(pitchAngle,0,0);
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        playerController = player;
        playerController.HoldInteractable(this);

        sizeChanger.Excute(coroutineChangePlanetSize(resizeFactor, resizeTime, EasingFunc.Easing.FunctionType.BackEaseOut));
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        playerController = null;
        sizeChanger.Excute(coroutineChangePlanetSize(1f, backTime));
    }
    public void BreakSpring(){
        useSpring = false;
        
        m_angularSpeed *= 0.5f;
        zeroSpringAngle = m_accumulateYaw;
    }
    public void FormSpring(){
        useSpring = true;
        zeroSpringAngle = m_accumulateYaw;
    }
    IEnumerator coroutineChangePlanetSize(float targetSize, float duration, EasingFunc.Easing.FunctionType easeType = EasingFunc.Easing.FunctionType.QuadEaseOut){
        var easeFunc = EasingFunc.Easing.GetFunctionWithTypeEnum(easeType);
        Vector3 initSize = pitchRotationTrans.localScale;

        yield return new WaitForLoop(duration, (t)=>{
            pitchRotationTrans.localScale = Vector3.LerpUnclamped(initSize, targetSize*Vector3.one, easeFunc(t));
        });
    }
}