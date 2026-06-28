using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using SimpleAudioSystem;

public class RangeDetection : MonoBehaviour
{
[Header("Basic")]
    [SerializeField] private SpriteRenderer rangeRenderer;
    [SerializeField] private SpriteRenderer boundryRenderer;
    [SerializeField] private Color fullfillColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private float testPerSec = 1;
    [SerializeField, ShowOnly] private bool allContained;

[Header("VFX")]
    [SerializeField] private ParticleSystem p_burst;

[Header("Animation")]
    [SerializeField] private Animator shapeAnimator;

[Header("Audio")]
    [SerializeField] private AudioData_SO sfxOnShapeContained;
    
    private List<ConnectBody> bodyHash;
    private Collider2D m_collider;
    private Color boundryInitColor;
    private bool isPunching;
    private bool hasEnlarged = false;
    private bool hasMaximized = false;
    [SerializeField] private bool hasIntersect = false;
    private int totalBodyCount;
    private float testTimer = 0;

    private const string SHAPE_CHANGE_ANIM = "ShapeChange";
    private const string SHAPE_EXPAND_ANIME = "ShapeExpand";
    private const string SHAPE_BLINK_BOOL = "IsBlink";
    private const string SHAPE_INTERSECT_BOOL = "IsIntersect";


    void Awake() => m_collider = GetComponent<Collider2D>();
    void Start()
    {
        bodyHash = new List<ConnectBody>();
        boundryInitColor = boundryRenderer.color;
    }
    void Update()
    {
        testTimer += Time.deltaTime * testPerSec;
        if(testTimer>=1)
        {
            testTimer = 0;
            bool newFlag = CheckIsAllShapeContained();
            if(newFlag != allContained)
            {
                allContained = newFlag;
            //Force Reset DOTWEEN
                rangeRenderer.transform.DOKill();
                rangeRenderer.transform.localScale = Vector3.one;
                isPunching = false;
                if(newFlag)
                    PunchScale(0.035f, 0.4f, 10);
                rangeRenderer.DOKill();
                rangeRenderer.DOColor(newFlag?fullfillColor:defaultColor, 0.2f);
                boundryRenderer.DOKill();
                boundryRenderer.DOColor(newFlag?Color.clear:boundryInitColor, 0.2f);
                if(newFlag)
                {
                    AudioManager.Instance.PlaySFX(sfxOnShapeContained.AudioKey, 1);
                    p_burst.Play();
                }
            }
        }
    }
    public void EnlargeDetection()
    {
        hasEnlarged = true;
        shapeAnimator.Play(SHAPE_CHANGE_ANIM);
    }
    public void MaximumDetection()
    {
        hasMaximized = true;
        shapeAnimator.Play(SHAPE_EXPAND_ANIME);
    }
    public void RangeAppear(float scale)
    {
        m_collider.enabled = false;
        transform.DOPunchScale(Vector3.one * scale, 0.25f).OnComplete(()=>
            m_collider.enabled = true
        );
    }
    public void SwitchBlink(bool isBlink) => shapeAnimator.SetBool(SHAPE_BLINK_BOOL, isBlink);
    public bool CheckIsAllShapeContained()
    {
        bool hashedContained = CheckIsHashedShapeContained();
        if(hasIntersect!=(!hashedContained))
        {
            hasIntersect = !hashedContained;
            shapeAnimator.SetBool(SHAPE_INTERSECT_BOOL, hasIntersect);
        }
        return bodyHash.Count>=totalBodyCount && hashedContained;
    }
    public bool CheckIsHashedShapeContained()
    {
        if(bodyHash.Count>0)
        {
            testTimer = 0;
            Vector2[] bodyPoints;
        //进行一次形状测试
            for(int i=bodyHash.Count-1; i>=0; i--)
            {
                bodyPoints = bodyHash[i].GetAllPoints();
                foreach(Vector2 point in bodyPoints)
                {
                    if(!m_collider.OverlapPoint(point))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        else
            return true;
    }
    public bool CheckPoint(Vector2 point) => m_collider.OverlapPoint(point);
    public void InitRangeDetect(int totalActiveBodyCount) => totalBodyCount = totalActiveBodyCount;
    public bool CanEnlarge() => !hasEnlarged;
    public bool CanMaximum() => !hasMaximized;
    void OnTriggerEnter2D(Collider2D other)
    {
        var body = other.GetComponentInParent<ConnectBody>();
        if(body != null && !bodyHash.Contains(body))
        {
            bodyHash.Add(body);
            PunchScale(-0.02f, 0.1f, 5);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        var body = other.GetComponentInParent<ConnectBody>();
        if(body != null && bodyHash.Contains(body))
        {
            bodyHash.Remove(body);
            PunchScale(0.02f, 0.1f, 5);
        }
    }
    void OnDisable()
    {
        rangeRenderer.transform.DOKill();
    }
    void PunchScale(float amount, float duration, int vibration)
    {
        if(!isPunching)
        {
            isPunching = true;
            rangeRenderer.transform.DOPunchScale(Vector3.one * amount, duration, vibration).OnComplete(()=>isPunching = false);
        }
    }
}