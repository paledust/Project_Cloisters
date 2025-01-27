using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class MirrorTextShowConstroller : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    private bool isFocusingText = false;

    private static readonly Color clearColor = new Color(1,1,1,0);
    private static readonly Color maxColor = new Color(1,1,1,0.1f);

    public void PlaceText(Vector3 pos, Quaternion rot)
    {
        tmp.transform.position = pos;
        tmp.transform.rotation = rot;
    }
    public void TintText(float focusFactor)
    {
        tmp.color = Color.Lerp(maxColor, clearColor, focusFactor);
    }
    public void ShowMirrorText(char textChar)
    {
        tmp.text = textChar.ToString();
    }
    public void HideMirrorText()
    {
        DOTween.Kill(tmp);
        tmp.DOColor(clearColor, 0.1f);
    }
    public void FocusText()
    {
        if(!isFocusingText)
        {
            isFocusingText = true;
        }
    }
    public void UnfocusText()
    {
        if(isFocusingText)
        {
            isFocusingText = false;
        }
    }
}