using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Playables;

public class IC_Meaningful : IC_Basic
{
    [System.Serializable]
    public class TextShownData
    {
        public char recieveChar;
        public Transform[] charPoses;
    }
    [SerializeField] private Clickable_ObjectRotator clickable_Mirror;
    [SerializeField] private List<TextShownData> textShownDatas;

    [Header("Diamond")]
    [SerializeField] private MirrorDiamond mirrorDiamond;
    [SerializeField] private ParticleSystem diamondFoundEffect;

    [Header("Ending")]
    [SerializeField] private Transform finalMirrorTrans;
    [SerializeField] private Transform mirrorRenderTrans;
    [SerializeField] private PlayableDirector director;

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        EventHandler.E_OnMirrorText += ShowText;
        EventHandler.E_OnMirrorDiamondFound += MirrorDiamondFoundHandler;
        clickable_Mirror.EnableHitbox();
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        EventHandler.E_OnMirrorText -= ShowText;
        EventHandler.E_OnMirrorDiamondFound -= MirrorDiamondFoundHandler;
        clickable_Mirror.DisableHitbox();
    }
    void ShowText(MirrorText mirrorText)
    {
        var data = textShownDatas.Find(x=>x.recieveChar==mirrorText.TextChar);
        if(data == null) return;

        textShownDatas.Remove(data);

        int count = 0;
        MirrorText tempText = mirrorText;
        foreach(var pos in data.charPoses)
        {
            if(count > 0)
            {
                tempText = Instantiate(mirrorText.gameObject).GetComponent<MirrorText>();
                tempText.transform.position = mirrorText.transform.position;
                tempText.transform.rotation = mirrorText.transform.rotation;
                tempText.CopyText(mirrorText);
            }
            float duration = Random.Range(2,2.5f);
            
            tempText.transform.DORotateQuaternion(Quaternion.identity, duration).SetEase(Ease.InOutQuad);
            tempText.transform.DOScale(pos.localScale, duration).SetEase(Ease.InOutQuad);
            tempText.transform.DOMove(pos.position, duration).SetEase(Ease.InOutQuad)
            .OnComplete(()=>{
                if(textShownDatas.Count == 0)
                {
                    mirrorDiamond.ActivateDiamond();
                }
            });
            count ++;
        }
    }
    void MirrorDiamondFoundHandler()
    {
        Quaternion finalRot;
        mirrorRenderTrans.SetParent(null);
        if(Vector3.Dot(finalMirrorTrans.up, mirrorRenderTrans.up) < 0)
        {
            finalRot = finalMirrorTrans.rotation * Quaternion.Euler(0,0,180);
        }
        else
            finalRot = finalMirrorTrans.rotation;
            
        mirrorRenderTrans.DORotateQuaternion(finalRot, 1f).SetEase(Ease.InOutQuad);

        diamondFoundEffect.Play();
        EventHandler.Call_OnFlushInput();
        EventHandler.Call_OnEndInteraction(this);
        StartCoroutine(coroutineEnding());
    }
    IEnumerator coroutineEnding()
    {
        yield return new WaitForSeconds(1f);
        director.Play();
    }
}