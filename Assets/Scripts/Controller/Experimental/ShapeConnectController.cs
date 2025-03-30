using UnityEngine;
using DG.Tweening;

public class ShapeConnectController : MonoBehaviour
{
    [SerializeField] private GameObject connectionBreakerPrefab;
    [SerializeField] private ParticleSystem p_collision;
    [SerializeField] private ParticleSystem p_break;
[Header("Connection")]
    [SerializeField] private float intersection = 0.1f;
    [SerializeField] private float connectDuration = 0.15f;

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
        EventHandler.Call_OnFlashInput();

        main.OnConnectionBuild();
        other.OnConnectionBuild();
        
        var mainBody = main.m_connectBody;
        var otherBody = other.m_connectBody;
    //Move Both rigid to a propery location
        Vector3 face;
        if(!mainBody.m_isSpherical && !otherBody.m_isSpherical)
            face = (main.normal - other.normal).normalized;
        else if(!mainBody.m_isSpherical)
            face = -main.normal;
        else if(!otherBody.m_isSpherical)
            face = other.normal;
        else
            face = (main.transform.position - other.transform.position).normalized;

        float angle = Vector2.SignedAngle(main.transform.up, face);
    
        Vector3 mid = main.transform.position + other.transform.position;
        if(!mainBody.m_isSpherical && !otherBody.m_isSpherical)
            mid = mid * 0.5f;
        else 
        {
            if(mainBody.m_isSpherical)
                mid = mid - face*mainBody.m_sphereRadius; 
            if(otherBody.m_isSpherical)
                mid = mid + face*otherBody.m_sphereRadius;
            mid = mid * 0.5f;
        }

        Vector3 offset = mid-(main.transform.position-face*mainBody.m_sphereRadius);
        offset = face*Vector3.Dot(offset, face);

    //Connect ConnectBody To Each Body
        mainBody.BuildConnection(otherBody);
        otherBody.BuildConnection(main.m_connectBody);

        main.m_connectBody.m_rigid.detectCollisions = false;
        other.m_connectBody.m_rigid.detectCollisions = false;
        
        var seq = DOTween.Sequence();
        seq.Append(mainBody.transform.DOMove(mainBody.transform.position + offset, connectDuration*0.5f).SetEase(Ease.InQuad))
        .Join(otherBody.transform.DOMove(otherBody.transform.position - offset, connectDuration*0.5f).SetEase(Ease.InQuad));

        if(!mainBody.m_isSpherical && !otherBody.m_isSpherical)
        {
            seq.Join(mainBody.transform.DORotateQuaternion(mainBody.transform.rotation *  Quaternion.Euler(0,0,angle), connectDuration*0.5f).SetEase(Ease.InQuad))
            .Join(otherBody.transform.DORotateQuaternion(otherBody.transform.rotation * Quaternion.Euler(0,0,-angle), connectDuration*0.5f).SetEase(Ease.InQuad));
        }

        seq.OnComplete(()=>
        {
        //Play Particles
            p_collision.transform.position = main.transform.position;
            p_collision.Play(true);
        //Seporate and create Joint
            var newSeq = DOTween.Sequence();
            newSeq.Append(mainBody.transform.DOMove(mainBody.transform.position - offset.normalized * intersection, connectDuration*0.5f)).SetEase(Ease.OutQuad)
            .Join(otherBody.transform.DOMove(otherBody.transform.position + offset.normalized * intersection, connectDuration*0.5f)).SetEase(Ease.OutQuad)
            .Join(mainBody.transform.DORotateQuaternion(mainBody.transform.rotation, connectDuration*0.5f).SetEase(Ease.OutQuad))
            .Join(otherBody.transform.DORotateQuaternion(otherBody.transform.rotation, connectDuration*0.5f).SetEase(Ease.OutQuad));

            newSeq.OnComplete(()=>{
            //Create Joint
                var joint =mainBody.m_rigid.gameObject.AddComponent<FixedJoint>();
                joint.connectedBody = otherBody.m_rigid;
            //Create JointBreaker
                Quaternion breakerRot;
                if(!mainBody.m_isSpherical)
                {
                    breakerRot = main.transform.rotation;
                }
                else if(!otherBody.m_isSpherical)
                {
                    breakerRot = other.transform.rotation;
                }
                else
                {
                    breakerRot = Quaternion.Euler(0,0,-Vector2.SignedAngle(main.transform.position - other.transform.position, Vector2.up));
                }
                    
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
                EventHandler.Call_OnBuildConnectionBreaker(jointBreaker);
            });
        });
    }
}