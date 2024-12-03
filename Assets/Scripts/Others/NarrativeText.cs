using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;

public class NarrativeText : MonoBehaviour
{
    [SerializeField] private Animation textFadeAnime;
    private const string fadeInName = "NarrativeFadeIn";
    public void FadeInText(){
        textFadeAnime.Play(fadeInName);
    }
}
