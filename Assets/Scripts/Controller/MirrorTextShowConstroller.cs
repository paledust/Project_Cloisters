using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using DG.Tweening;

public class MirrorTextShowConstroller : MonoBehaviour
{
    [SerializeField] private TextMeshPro tmp;
    public void PlaceText(Vector3 pos, Quaternion rot)
    {
        tmp.transform.position = pos;
        tmp.transform.rotation = rot;
    }
    public void ShowMirrorText(char textChar)
    {
        tmp.text = textChar.ToString();
        DOTween.Kill(tmp);
        tmp.DOColor(new Color(1,1,1,0.5f), 2f);
    }
    public void HideMirrorText()
    {
        DOTween.Kill(tmp);
        tmp.DOColor(new Color(1,1,1,0), 0.1f);
    }
}