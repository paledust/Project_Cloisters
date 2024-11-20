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

    public float m_angularSpeed{get; private set;}
    public bool m_isControlling{get{return playerController!=null;}}

    private float initDepth;
    private float lastAngle;
    private float verticalAngle;
    private Vector3 hitPos;
    private Camera mainCam;
    private PlayerController playerController;
    private CoroutineExcuter sizeChanger;

    void Start(){
        mainCam = Camera.main;
        verticalAngle = 0;
        sizeChanger = new CoroutineExcuter(this);
    }
    void Update(){
        if(playerController!=null){
            Vector3 cursorPos = playerController.PointerScrPos;
            cursorPos.z = initDepth;
            cursorPos = mainCam.ScreenToWorldPoint(cursorPos);
            
            float angle = (cursorPos - hitPos).x * dragStrength;
            if(Time.deltaTime>0) m_angularSpeed = Mathf.Lerp(m_angularSpeed, Mathf.Clamp((angle-lastAngle)/Time.deltaTime, -maxAngularSpeed, maxAngularSpeed), Time.deltaTime*controllingAngularLerp);
            lastAngle = angle;

            float targetAngle = (cursorPos-hitPos).y * dragStrength * 0.5f;
            targetAngle = Mathf.Clamp(targetAngle, -maxVerticalAngle, maxVerticalAngle);
            verticalAngle = Mathf.Lerp(verticalAngle, targetAngle, Time.deltaTime*controllingAngularLerp);
        }
        else{
            verticalAngle = Mathf.Lerp(verticalAngle, 0, Time.deltaTime*releaseAngularLerp);
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, idleAngularSpeed, Time.deltaTime*releaseAngularLerp);
            if(Mathf.Abs(m_angularSpeed-idleAngularSpeed)<=0.01f) m_angularSpeed = idleAngularSpeed;
        }
    }
    void FixedUpdate(){
        planetTrans.Rotate(Vector3.forward, -m_angularSpeed*Time.fixedDeltaTime, Space.Self);
        planetAxisTrans.localRotation = Quaternion.Euler(verticalAngle,0,0);
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        lastAngle = 0;
        playerController = player;
        playerController.HoldInteractable(this);
        this.hitPos = hitPos;
        initDepth = Camera.main.WorldToScreenPoint(this.hitPos).z;

        sizeChanger.Excute(coroutineChangePlanetSize(1.02f, 0.4f, EasingFunc.Easing.FunctionType.BackEaseOut));
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        playerController = null;
        sizeChanger.Excute(coroutineChangePlanetSize(1f, 0.5f));
    }
    IEnumerator coroutineChangePlanetSize(float targetSize, float duration, EasingFunc.Easing.FunctionType easeType = EasingFunc.Easing.FunctionType.QuadEaseOut){
        var easeFunc = EasingFunc.Easing.GetFunctionWithTypeEnum(easeType);
        Vector3 initSize = planetAxisTrans.localScale;

        yield return new WaitForLoop(duration, (t)=>{
            planetAxisTrans.localScale = Vector3.LerpUnclamped(initSize, targetSize*Vector3.one, easeFunc(t));
        });
    }
}