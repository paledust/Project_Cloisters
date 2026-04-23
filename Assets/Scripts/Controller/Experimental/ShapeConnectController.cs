using UnityEngine;
using DG.Tweening;
using System.Collections.Generic;

public class ShapeConnectController : MonoBehaviour
{
    [SerializeField] private GameObject connectionBreakerPrefab;
    [SerializeField] private ParticleSystem p_collision;
    [SerializeField] private ParticleSystem p_break;

[Header("Connection")]
    [SerializeField] private float intersection = 0.1f;
    [SerializeField] private float connectDuration = 0.15f;

    private Dictionary<ConnectBody, ConnectBody[]> bodyGraph = new Dictionary<ConnectBody, ConnectBody[]>();

    void Awake()
    {
        EventHandler.E_OnShapeConnect += OnShapeConnect;
        EventHandler.E_OnBreakConnectionBreaker += OnShapeBreak;
    }
    void OnDestroy()
    {
        EventHandler.E_OnShapeConnect -= OnShapeConnect;
        EventHandler.E_OnBreakConnectionBreaker -= OnShapeBreak;
    }
    void OnShapeBreak(Clickable_ConnectionBreaker connectionBreaker, Vector3 breakPoint)
    {
        p_break.transform.position = breakPoint;
        p_break.Play(true);
    }
    void OnShapeConnect(ConnectTrigger main, ConnectTrigger other)
    {
        EventHandler.Call_OnFlushInput();

        main.OnConnectionBuild();
        other.OnConnectionBuild();
        
        var mainBody = main.m_connectBody;
        var otherBody = other.m_connectBody;
    //Move Both rigid to a propery location
        Vector3 face;
        face = (main.normal - other.normal).normalized;
        float angle = Vector2.SignedAngle(main.normal, face);
    
        Vector3 mid = main.transform.position + other.transform.position;
        mid = mid * 0.5f;

        Vector3 offset = mainBody.transform.position - main.transform.position;
        offset = Quaternion.Euler(0,0,angle) * offset;
        Vector3 otherOffset = otherBody.transform.position - other.transform.position;
        otherOffset = Quaternion.Euler(0,0,-angle) * otherOffset;

    //Connect ConnectBody To Each Body
        mainBody.BuildConnection(otherBody);
        otherBody.BuildConnection(main.m_connectBody);

        main.m_connectBody.m_rigid.isKinematic = true;
        other.m_connectBody.m_rigid.isKinematic = true;
        
        Vector3 targetPos = mid + offset - face * intersection;
        Vector3 otherTargetPos = mid + otherOffset + face * intersection;
    
    //Separate and Rotate to face each other
        var seq = DOTween.Sequence();
        seq.Append(mainBody.transform.DOMove(mid - face*0.4f + offset, connectDuration*0.25f).SetEase(Ease.OutCirc))
        .Join(mainBody.transform.DORotateQuaternion(mainBody.transform.rotation *  Quaternion.Euler(0,0,angle), connectDuration*0.5f).SetEase(Ease.OutCirc))
        .Join(otherBody.transform.DOMove(mid + face*0.4f + otherOffset, connectDuration*0.25f).SetEase(Ease.OutCirc))
        .Join(otherBody.transform.DORotateQuaternion(otherBody.transform.rotation * Quaternion.Euler(0,0,-angle), connectDuration*0.5f).SetEase(Ease.OutCirc));
    //Combine Together
        seq.Append(mainBody.transform.DOMove(targetPos, connectDuration*0.25f).SetEase(Ease.InCirc))
        .Join(otherBody.transform.DOMove(otherTargetPos, connectDuration*0.25f).SetEase(Ease.InCirc))
        .OnComplete(()=>
        {
        //Play Particles
            p_collision.transform.position = main.transform.position;
            p_collision.Play(true);
        //Seporate and create Joint
            mainBody.transform.position = targetPos;
            otherBody.transform.position = otherTargetPos;
            mainBody.m_rigid.position = targetPos;
            otherBody.m_rigid.position = otherTargetPos;
        //Create Joint
            var joint = mainBody.m_rigid.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = otherBody.m_rigid;
            joint.enableCollision = false;
        //Create JointBreaker
            Quaternion breakerRot;
            if(!mainBody.m_isSpherical)
                breakerRot = main.transform.rotation;
            else if(!otherBody.m_isSpherical)
                breakerRot = other.transform.rotation;
            else
                breakerRot = Quaternion.Euler(0,0,-Vector2.SignedAngle(main.transform.position - other.transform.position, Vector2.up));
            
            var jointBreaker = Instantiate(connectionBreakerPrefab, mid, breakerRot).GetComponent<Clickable_ConnectionBreaker>();
            jointBreaker.transform.position = mid;
            jointBreaker.transform.rotation = breakerRot;
            Vector3 scale = main.transform.localScale;
            scale.y = 1f;
            scale.z = 3f;
            jointBreaker.transform.localScale = scale;
            jointBreaker.transform.parent = mainBody.transform;
            jointBreaker.InitConnection(joint, main, other);
            main.m_connectBody.m_rigid.detectCollisions = true;
            other.m_connectBody.m_rigid.detectCollisions = true;
            main.m_connectBody.m_rigid.isKinematic = false;
            other.m_connectBody.m_rigid.isKinematic = false;

            EventHandler.Call_OnBuildConnectionBreaker(jointBreaker);
        });
    }
}