using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_Planet : Basic_Clickable
{
    [SerializeField] private Transform planetTrans;
[Header("Control")]
    [SerializeField] private float dragStrength = 1f;
    [SerializeField] private float idleAngularSpeed = 2f;
    [SerializeField] private float maxAngularSpeed = 200f;
[Header("AngularSpeed Lerp")]
    [SerializeField] private float controllingAngularLerp = 10f;
    [SerializeField] private float releaseAngularLerp = 1f;

    public float m_angularSpeed{get; private set;}

    private float initDepth;
    private float lastAngle;
    private Vector3 hitPos;
    private Vector3 rotateAxis;
    private PlayerController playerController;
    private Camera mainCam;

    void Start(){
        mainCam = Camera.main;
        rotateAxis = Vector3.forward;
    }
    void Update(){
        if(playerController!=null){
            Vector3 cursorPos = playerController.PointerScrPos;
            cursorPos.z = initDepth;
            cursorPos = mainCam.ScreenToWorldPoint(cursorPos);
            // rotateAxis = Vector3.Lerp(rotateAxis, Vector3.Cross(Vector3.back, cursorPos - initPos).normalized, Time.deltaTime * controllingAngularLerp);
            
            float angle = (cursorPos - hitPos).x * dragStrength;
            if(Time.deltaTime>0) m_angularSpeed = Mathf.Lerp(m_angularSpeed, Mathf.Clamp((angle-lastAngle)/Time.deltaTime, -maxAngularSpeed, maxAngularSpeed), Time.deltaTime*controllingAngularLerp);

            lastAngle = angle;
        }
        else{
            // rotateAxis = Vector3.Lerp(rotateAxis, Vector3.up, Time.deltaTime*releaseAngularLerp);
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, idleAngularSpeed, Time.deltaTime*releaseAngularLerp);
            if(Mathf.Abs(m_angularSpeed-idleAngularSpeed)<=0.01f) m_angularSpeed = idleAngularSpeed;
        }


    }
    void FixedUpdate(){
        planetTrans.Rotate(rotateAxis, -m_angularSpeed*Time.fixedDeltaTime, Space.Self);
    }
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        lastAngle = 0;
        playerController = player;
        playerController.HoldInteractable(this);
        this.hitPos = hitPos;
        initDepth = Camera.main.WorldToScreenPoint(this.hitPos).z;
    }
    public override void OnRelease(PlayerController player)
    {
        base.OnRelease(player);
        playerController = null;
    }
}