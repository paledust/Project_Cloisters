using UnityEngine;
using DG.Tweening;

public class Blinker : MonoBehaviour
{
    [SerializeField] private SpriteRenderer bouncerBlink;
    [SerializeField] private SpriteRenderer blinkRender;
    void Update()
    {
        blinkRender.color = bouncerBlink.color;
    }
}
