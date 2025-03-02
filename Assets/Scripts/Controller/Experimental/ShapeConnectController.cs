using UnityEngine;
using DG.Tweening;

public class ShapeConnectController : MonoBehaviour
{
    [SerializeField] private GameObject connectionBreakerPrefab;
[Header("Connection")]
    [SerializeField] private float intersection = 0.1f;
    [SerializeField] private float connectDuration = 0.15f;

    void Awake()
    {
        EventHandler.E_OnShapeConnect += OnShapeConnect;
    }
    void OnDestroy()
    {
        EventHandler.E_OnShapeConnect -= OnShapeConnect;
    }
    void OnShapeConnect(ConnectTrigger main, ConnectTrigger other)
    {
        EventHandler.Call_OnFlashInput();

        main.OnConnectionBuild();
        other.OnConnectionBuild();
        
        var mainBody = main.m_connectBody;
        var otherBody = other.m_connectBody;
    //Move Both rigid to a propery location
        Vector3 face = (main.transform.up - other.transform.up).normalized;
        float angle = Vector2.SignedAngle(main.transform.up, face);
    
        Vector3 mid = (main.transform.position + other.transform.position) * 0.5f;
        Vector3 offset = mid-main.transform.position;
        offset = face*Vector3.Dot(offset, face);

    //Connect ConnectBody To Each Body
        mainBody.BuildConnection(otherBody);
        otherBody.BuildConnection(main.m_connectBody);

        main.m_connectBody.m_rigid.detectCollisions = false;
        other.m_connectBody.m_rigid.detectCollisions = false;
        var seq = DOTween.Sequence();
        seq.Append(mainBody.transform.DORotateQuaternion(mainBody.transform.rotation *  Quaternion.Euler(0,0,angle), connectDuration).SetEase(Ease.InOutQuad))
        .Join(mainBody.transform.DOMove(mainBody.transform.position + offset - face*intersection, connectDuration).SetEase(Ease.InOutQuad))
        .Join(otherBody.transform.DORotateQuaternion(otherBody.transform.rotation * Quaternion.Euler(0,0,-angle), connectDuration).SetEase(Ease.InOutQuad))
        .Join(otherBody.transform.DOMove(otherBody.transform.position - offset + face*intersection, connectDuration).SetEase(Ease.InOutQuad))
        .OnComplete(()=>{
        //Create Joint
            var joint =mainBody.m_rigid.gameObject.AddComponent<FixedJoint>();
            joint.connectedBody = otherBody.m_rigid;
        //Create JointBreaker
            var jointBreaker = Instantiate(connectionBreakerPrefab, mid, main.transform.rotation).GetComponent<Clickable_ConnectionBreaker>();
            jointBreaker.transform.position = mid;
            jointBreaker.transform.rotation = main.transform.rotation;
            Vector3 scale = main.transform.localScale;
            scale.y = 1f;
            scale.z = 3f;
            jointBreaker.transform.localScale = scale;
            jointBreaker.transform.parent = mainBody.transform;
            jointBreaker.InitConnection(joint, main, other);

            main.m_connectBody.m_rigid.detectCollisions = true;
            other.m_connectBody.m_rigid.detectCollisions = true;
        });
    }
}