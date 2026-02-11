using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GeoTextController : MonoBehaviour
{
[Header("Text Fly")]
    [SerializeField] private IC_Stylized ic_stylized;
    [SerializeField] private Transform planetCenter;
    [SerializeField] private GameObject[] textObjs;
    [SerializeField] private Transform[] finalTrans;
    [SerializeField] private AnimationCurve flyCurve;
    [SerializeField] private AnimationCurve scaleCurve;
    [SerializeField] private AnimationCurve finalizeCurve;
    [SerializeField] private float flyTime = 0.5f;
    [SerializeField] private float flyRadius = 2;
    [SerializeField, Range(0.00001f, 0.99999f)] private float eclipseRatio = 1;
[Header("Text Shift")]
    [SerializeField] private float explodeShift = 100;
    [SerializeField] private Clickable_ObjectRotator clickablePlanet;
    [SerializeField] private Vector2 textShiftRange;
    [SerializeField] private float textShiftScale = 0.2f;
    [SerializeField] private float textShiftLerpSpeed = 2;

    private bool shiftExploding = false;
    private float shiftFactor;
    private Stack<ShiftData> shiftDataList = new Stack<ShiftData>();

    private struct ShiftData{
        public Transform textTrans;
        public Vector3 originalPos;
        public float shiftDistance;
        public float shiftOffset;
        public void UpdateShiftTrans(Vector3 centerPos, float t)
        {
            textTrans.position = originalPos + (originalPos - centerPos).normalized * shiftDistance * (t-shiftOffset);
        }
    }
    private int textIndex = 0;
    private int[] dirIndex;
    void Awake(){
        dirIndex = new int[textObjs.Length];
        for(int i=0; i<dirIndex.Length; i++){
            dirIndex[i] = i;
        }
        Service.Shuffle(ref dirIndex);
    }
    void Update()
    {
        float targetShiftFactor = clickablePlanet.m_angularSpeed * textShiftScale;
        if(!shiftExploding)
            shiftFactor = Mathf.LerpUnclamped(shiftFactor, targetShiftFactor, Time.deltaTime*textShiftLerpSpeed);
        foreach(var shiftText in shiftDataList){
            shiftText.UpdateShiftTrans(planetCenter.position, shiftFactor);
        }
    }
    public void PopText(){
        if(textIndex>=textObjs.Length) return;

        StartCoroutine(coroutineLerpShiftFactor(explodeShift, 1f));
        var textTrans = textObjs[textIndex].transform;
        float unitAngle = 360f/(0f+textObjs.Length);
        float angle = unitAngle*dirIndex[textIndex]+Random.Range(-10f, 10f);
        float ejectLength = EclipseDeform.DeformedRadius(flyRadius*Random.Range(0.9f, 1.1f), eclipseRatio, angle*Mathf.Deg2Rad);
        Vector3 ejectDir = Quaternion.Euler(0,0,angle)*Vector3.right;

        textTrans.position = planetCenter.position;
        textTrans.localScale = Vector3.one * 25;
        textTrans.rotation = Quaternion.Euler(Random.Range(-10, 10), Random.Range(-10, 10)-180, Random.Range(-50, 50));
        textTrans.gameObject.SetActive(true);

        Vector3 finalPos = planetCenter.position + ejectDir*ejectLength + Vector3.back * 0.3f;

        textTrans.DOMove(finalPos, flyTime).SetEase(flyCurve).OnComplete(()=>{
            ShiftData shiftData = new ShiftData(){
                textTrans = textTrans,
                originalPos = textTrans.position,
                shiftDistance = textShiftRange.GetRndValueInVector2Range(),
                shiftOffset = shiftFactor
            };

            shiftDataList.Push(shiftData);
        });
        textTrans.DOScale(Vector3.one*50, flyTime).SetEase(scaleCurve).OnComplete(() =>
        {
            textTrans.GetComponent<Clickable_Stylized>().enabled = true;
            textTrans.GetComponent<Clickable_Stylized>().EnableHitbox();
        });
        textTrans.DORotateQuaternion(Quaternion.Euler(Random.Range(-20,20), Random.Range(-20,20), Random.Range(-80,80)+360)*textTrans.rotation, flyTime).SetEase(flyCurve);
        textIndex ++;

        if(textIndex>=textObjs.Length){
            shiftDataList.Clear();
            ic_stylized.OnAllTextOut();
        }
    }
    public void PutTextTogether(){
        float delay = 0;
        Service.Shuffle(ref dirIndex);

        for(int i=0; i<textObjs.Length; i++){
            int index = dirIndex[i];
            textObjs[index].transform.DOKill();
            textObjs[index].transform.DOMove(finalTrans[index].position,Random.Range(1f,1.2f)).SetEase(finalizeCurve).SetDelay(delay, false);
            textObjs[index].transform.DORotateQuaternion(finalTrans[index].rotation, Random.Range(1f,1.2f)).SetEase(finalizeCurve).SetDelay(delay, false);
            delay += Random.Range(0.0f, 0.15f);
        }
    }
    IEnumerator coroutineLerpShiftFactor(float targetValue, float duration){
        shiftExploding = true;
        float initFactor = shiftFactor;
        yield return new WaitForLoop(duration, (t)=>{
            shiftFactor = Mathf.Lerp(initFactor, targetValue, EasingFunc.Easing.QuadEaseOut(t));
        });
        shiftExploding = false;
    }
}