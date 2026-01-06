using TMPro;
using UnityEngine;

public class NarrativeText : MonoBehaviour
{
    [SerializeField] private Animation textFadeAnime;
    [SerializeField] private TextMeshPro[] texts;
    public void FadeInText(string content){
        foreach(var text in texts){
            text.text = content;
        }
        textFadeAnime.Play();
    }
}
