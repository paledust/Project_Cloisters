using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class GeoTextController : MonoBehaviour
{
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

    private int textIndex = 0;
    private int[] dirIndex;
    void Awake(){
        dirIndex = new int[textObjs.Length];
        for(int i=0; i<dirIndex.Length; i++){
            dirIndex[i] = i;
        }
        Service.Shuffle(ref dirIndex);
    }

    public void ShowText(){
        if(textIndex>=textObjs.Length) return;

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

        textTrans.DOMove(finalPos, flyTime).SetEase(flyCurve);
        textTrans.DOScale(Vector3.one*50, flyTime).SetEase(scaleCurve);
        textTrans.DORotateQuaternion(Quaternion.Euler(Random.Range(-20,20), Random.Range(-20,20), Random.Range(-80,80)+360)*textTrans.rotation, flyTime).SetEase(flyCurve);
        textIndex ++;

        if(textIndex>=textObjs.Length){
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
}