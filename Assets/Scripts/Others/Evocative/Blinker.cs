using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Blinker : MonoBehaviour
{
    [SerializeField] private Bouncer bouncer;
    [SerializeField] private SpriteRenderer blinkRender;
    void Start()
    {
        bouncer.onBounce += BounceHandle;      
    }

    void BounceHandle(BounceBall bounceBall)
    {
        blinkRender.DOKill();
        blinkRender.DOFade(1, 0.1f).OnComplete(() => blinkRender.DOFade(0, 0.05f));
    }
}
