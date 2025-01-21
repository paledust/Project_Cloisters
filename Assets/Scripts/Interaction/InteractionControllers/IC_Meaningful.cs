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
        public TextMeshPro[] showingTMP;
    }
    [SerializeField] private Clickable_ObjectRotator clickable_Mirror;
    [SerializeField] private TextShownData[] textShownDatas;
    private Dictionary<char, TextMeshPro[]> showingDict;
    void Awake()
    {
        showingDict = new Dictionary<char, TextMeshPro[]>();
        foreach(var item in textShownDatas)
        {
            showingDict.Add(item.recieveChar[0], item.showingTMP);
            foreach(var text in item.showingTMP)
            {
                text.color = new Color(1,1,1,0);
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
            foreach(var value in showingDict[c])
            {
                value.DOColor(Color.white, 2f).SetEase(Ease.OutQuad);
            }
            showingDict.Remove(c);
        }

        if(showingDict.Count == 0)
        {
            EventHandler.Call_OnEndInteraction(this);
        }
    }
}