using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using SimpleAudioSystem;
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
    [Header("Meaningful")]
    [SerializeField] private Clickable_ObjectRotator clickable_Mirror;
    [SerializeField] private List<TextShownData> textShownDatas;

    [Header("Diamond")]
    [SerializeField] private MirrorDiamond mirrorDiamond;
    [SerializeField] private ParticleSystem diamondFoundEffect;

    [Header("Ending")]
    [SerializeField] private Transform finalMirrorTrans;
    [SerializeField] private Transform mirrorRenderTrans;
    [SerializeField] private PlayableDirector director;
    [SerializeField] private List<MirrorText> textList;
    [SerializeField] private AudioData_SO glowSFX;
    private bool IsAllTextFound = false;

    protected override void OnInteractionEnter()
    {
        base.OnInteractionEnter();
        IsAllTextFound = false;
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
        foreach(var pos in data.charPoses)
        {
            var tempText = mirrorText;
            if(count > 0)
            {
                tempText = Instantiate(mirrorText.gameObject, mirrorText.transform.parent).GetComponent<MirrorText>();
                tempText.transform.position = mirrorText.transform.position;
                tempText.transform.rotation = mirrorText.transform.rotation;
                tempText.CopyText(mirrorText);
                textList.Insert(5, tempText);
            }
            float duration = Random.Range(2,2.5f);
            
            tempText.transform.DORotateQuaternion(Quaternion.identity, duration).SetEase(Ease.InOutQuad);
            tempText.transform.DOScale(pos.localScale, duration).SetEase(Ease.InOutQuad);
            tempText.transform.DOMove(pos.position, duration).SetEase(Ease.InOutQuad)
            .OnComplete(()=>{
                if(textShownDatas.Count == 0 && !IsAllTextFound)
                {
                    IsAllTextFound = true;
                    StartCoroutine(coroutineDiamond());
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
    IEnumerator coroutineDiamond()
    {
        yield return new WaitForSeconds(3f);
        AudioManager.Instance.PlaySFX(glowSFX.AudioKey, 0.5f);
        foreach(var text in textList)
        {
            text.transform.DOPunchScale(Vector3.one * .005f, 1.5f, 1);
            text.TurnOnGlow();
            yield return new WaitForSeconds(0.12f);
        }
        yield return new WaitForSeconds(0.5f);
        foreach(var text in textList)
        {
            text.TurnOffGlow();
        }
        yield return new WaitForSeconds(.5f);
        mirrorDiamond.ActivateDiamond();
    }
    [ContextMenu("Editor_TestDiamond")]
    public void TestDiamond()
    {
        StartCoroutine(coroutineDiamond());
    }
}