using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_Planet : Basic_Clickable
{
    [SerializeField] private Transform planetAxisTrans;
    [SerializeField] private Transform planetTrans;
[Header("Control")]
    [SerializeField] private float dragStrength = 1f;
    [SerializeField] private float idleAngularSpeed = 2f;
    [SerializeField] private float maxAngularSpeed = 200f;
    [SerializeField] private float maxVerticalAngle = 20;
[Header("AngularSpeed Lerp")]
    [SerializeField] private float controllingAngularLerp = 10f;
    [SerializeField] private float releaseAngularLerp = 1f;
[Header("Spring")]
    [SerializeField] private bool useSpring = false;
    [SerializeField] private float springFactor = 2f;

    public float m_angularSpeed{get; private set;}
    public float m_accumulateYaw{get; private set;}
    public bool m_isControlling{get{return playerController!=null;}}

    private float targetVerticalAngle;
    private float verticalAngle;
    private float zeroSpringAngle = 0;
    private PlayerController playerController;
    private CoroutineExcuter sizeChanger;

    void Start(){
        verticalAngle = 0;
        sizeChanger = new CoroutineExcuter(this);
    }
    void Update(){
        if(playerController!=null){
            Vector2 delta = playerController.PointerDelta;
            
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, Mathf.Clamp(delta.x * dragStrength, -maxAngularSpeed, maxAngularSpeed), Time.deltaTime*controllingAngularLerp);

            targetVerticalAngle += delta.y * dragStrength * 0.5f * Time.deltaTime;
            targetVerticalAngle = Mathf.Clamp(targetVerticalAngle, -maxVerticalAngle, maxVerticalAngle);
            verticalAngle = Mathf.Lerp(verticalAngle, targetVerticalAngle, Time.deltaTime*controllingAngularLerp);
        }
        else{
            verticalAngle = Mathf.Lerp(verticalAngle, 0, Time.deltaTime*releaseAngularLerp);
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, idleAngularSpeed, Time.deltaTime*releaseAngularLerp);
            if(Mathf.Abs(m_angularSpeed-idleAngularSpeed)<=0.01f) m_angularSpeed = idleAngularSpeed;
        }
    }
    void FixedUpdate(){
        if(useSpring)m_angularSpeed += (m_accumulateYaw-zeroSpringAngle)*springFactor*Time.fixedDeltaTime;
        m_accumulateYaw -= m_angularSpeed*Time.fixedDeltaTime;
        planetTrans.Rotate(Vector3.forward, -m_angularSpeed*Time.fixedDeltaTime, Space.Self);
        planetAxisTrans.localRotation = Quaternion.Euler(verticalAngle,0,0);
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        playerController = player;
        playerController.HoldInteractable(this);

        sizeChanger.Excute(coroutineChangePlanetSize(1.02f, 0.4f, EasingFunc.Easing.FunctionType.BackEaseOut));
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        playerController = null;
        sizeChanger.Excute(coroutineChangePlanetSize(1f, 0.5f));
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
        Vector3 initSize = planetAxisTrans.localScale;

        yield return new WaitForLoop(duration, (t)=>{
            planetAxisTrans.localScale = Vector3.LerpUnclamped(initSize, targetSize*Vector3.one, easeFunc(t));
        });
    }
}