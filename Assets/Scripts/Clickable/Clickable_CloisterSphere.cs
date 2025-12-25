using System.Collections;
using UnityEngine;

public class Clickable_CloisterSphere : Basic_Clickable
{
    [SerializeField] private Transform rotateTrans;
[Header("Control")]
    [SerializeField] private float dragStrength = 1f;
    [SerializeField] private float maxAngularSpeed = 200f;
[Header("Resize")]
    [SerializeField] private float resizeFactor = 1.02f;
    [SerializeField] private float resizeTime = 0.4f;
    [SerializeField] private float backTime = 0.5f;
[Header("AngularSpeed Lerp")]
    [SerializeField] private float controllingAngularLerp = 10f;
    [SerializeField] private float releaseAngularLerp = 1f;

    public float m_angularSpeed{get; private set;}

    private PlayerController playerController;
    private CoroutineExcuter sizeChanger;

    void Start()
    {
        sizeChanger = new CoroutineExcuter(this);
    }
    void Update()
    {
        if(playerController!=null){
            Vector2 delta = playerController.PointerDelta;
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, Mathf.Clamp(delta.x * dragStrength, -maxAngularSpeed, maxAngularSpeed), Time.deltaTime*controllingAngularLerp);
        }
        else{
            m_angularSpeed = Mathf.Lerp(m_angularSpeed, 0, Time.deltaTime*releaseAngularLerp);
            if(Mathf.Abs(m_angularSpeed)<=0.01f) m_angularSpeed = 0;
        }
    }
    void FixedUpdate()
    {
        rotateTrans.Rotate(Vector3.up, -m_angularSpeed*Time.fixedDeltaTime, Space.Self);
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
    public override void OnHover(PlayerController player)
    {
        base.OnHover(player);
        UI_Manager.Instance.ChangeCursorColor(true);
    }
    public override void OnExitHover()
    {
        base.OnExitHover();
        UI_Manager.Instance.ChangeCursorColor(false);
    }
    IEnumerator coroutineChangePlanetSize(float targetSize, float duration, EasingFunc.Easing.FunctionType easeType = EasingFunc.Easing.FunctionType.QuadEaseOut){
        var easeFunc = EasingFunc.Easing.GetFunctionWithTypeEnum(easeType);
        Vector3 initSize = rotateTrans.localScale;

        yield return new WaitForLoop(duration, (t)=>{
            rotateTrans.localScale = Vector3.LerpUnclamped(initSize, targetSize*Vector3.one, easeFunc(t));
        });
    }
}
