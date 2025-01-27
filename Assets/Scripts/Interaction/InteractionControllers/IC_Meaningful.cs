using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

public class IC_Meaningful : IC_Basic
{
    [System.Serializable]
    public struct TextShownData
    {
        public string recieveChar;
        public SpriteRenderer[] spriteRenderers;
    }
    [SerializeField] private Clickable_ObjectRotator clickable_Mirror;
    [SerializeField] private TextShownData[] textShownDatas;
    private Dictionary<char, SpriteRenderer[]> showingDict;
    void Awake()
    {
        showingDict = new Dictionary<char, SpriteRenderer[]>();
        foreach(var item in textShownDatas)
        {
            showingDict.Add(item.recieveChar[0], item.spriteRenderers);
            foreach(var spriterenderer in item.spriteRenderers)
            {
                spriterenderer.color = new Color(1,1,1,0);
            }
        }
    }
    protected override void OnInteractionStart()
    {
        base.OnInteractionStart();
        EventHandler.E_OnMirrorText += ShowText;
        clickable_Mirror.EnableHitbox();
    }
    protected override void OnInteractionEnd()
    {
        base.OnInteractionEnd();
        EventHandler.E_OnMirrorText -= ShowText;
        clickable_Mirror.DisableHitbox();
    }
    void ShowText(char c)
    {
        if(showingDict.ContainsKey(c))
        {
            foreach(var spriterenderer in showingDict[c])
            {
                spriterenderer.DOKill();
                spriterenderer.DOFade(1, 2f).SetEase(Ease.InOutQuad)
                .OnComplete(()=>{
                    showingDict.Remove(c);
                    if(showingDict.Count == 0)
                    {
                    }
                });
            }
        }
    }
}