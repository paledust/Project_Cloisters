using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class NarrativeText : MonoBehaviour
{
    [SerializeField] private Animation textFadeAnime;
    public void FadeInText(){
        textFadeAnime.Play();
        StartCoroutine(CommonCoroutine.delayAction(()=>{
            gameObject.SetActive(false);
        }, textFadeAnime.clip.length));
    }
}
