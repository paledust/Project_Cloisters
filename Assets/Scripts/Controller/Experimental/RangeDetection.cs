using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class RangeDetection : MonoBehaviour
{
[Header("Basic")]
    [SerializeField] private SpriteRenderer rangeRenderer;
    [SerializeField] private float testPerSec = 1;
    [SerializeField] private Color fullfillColor;
    [SerializeField] private Color defaultColor;
    [SerializeField] private bool isRange;
[Header("Animation")]
    [SerializeField] private Animation anime;
    private List<ConnectBody> bodyHash;
    private Collider2D m_collider;
    private bool isPunching;
    private bool hasEnlarged = false;
    private int totalBodyCount;
    private float testTimer = 0;
    public int BodyCount => bodyHash.Count;


    void Awake() => m_collider = GetComponent<Collider2D>();
    void Start()
    {
        bodyHash = new List<ConnectBody>();
    }
    void Update()
    {
        testTimer += Time.deltaTime * testPerSec;
        if(testTimer>=1)
        {
            testTimer = 0;
            bool newFlag = CheckRange();
            if(newFlag != isRange)
            {
                isRange = newFlag;
                PunchCondition(newFlag?fullfillColor:defaultColor);
            }
        }
    }
    public void EnlargeDetection()
    {
        hasEnlarged = true;
        anime.Play();
    }
    public void RangeAppear(float scale)
    {
        m_collider.enabled = false;
        transform.localScale = Vector3.zero;
        transform.DOScale(scale, 0.4f).SetEase(Ease.OutBack, 2).OnComplete(()=>
            m_collider.enabled = true
        );
    }
    public bool CheckRange()
    {
        if(bodyHash.Count>=totalBodyCount)
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
        }
        return false;
    }
    public void InitRangeDetect(int totalActiveBodyCount) => totalBodyCount = totalActiveBodyCount;
    public bool CanEnlarge() => !hasEnlarged;
    void OnTriggerEnter2D(Collider2D other)
    {
        var body = other.GetComponentInParent<ConnectBody>();
        if(body != null && !bodyHash.Contains(body))
        {
            bodyHash.Add(body);
            PunchScale(-0.035f);
        }
    }
    void OnTriggerExit2D(Collider2D other)
    {
        var body = other.GetComponentInParent<ConnectBody>();
        if(body != null && bodyHash.Contains(body))
        {
            bodyHash.Remove(body);
            PunchScale(0.05f);
        }
    }
    void PunchScale(float amount)
    {
        if(!isPunching)
        {
            isPunching = true;
            rangeRenderer.transform.DOPunchScale(Vector3.one * amount, 0.4f, 1).OnComplete(()=>isPunching = false);
        }
    }
    void PunchCondition(Color targetColor)
    {
    //Force Reset DOTWEEN
        rangeRenderer.transform.DOKill();
        rangeRenderer.transform.localScale = Vector3.one;
        isPunching = false;
        PunchScale(0.05f);
        rangeRenderer.DOKill();
        rangeRenderer.DOColor(targetColor, 0.2f);
    }
}