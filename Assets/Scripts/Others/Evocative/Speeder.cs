using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Speeder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer speederRenderer;
    [SerializeField] private float force = 2f;

    private float initAlpha = 0;
    private Rigidbody targetBody;

    void Start()
    {
        initAlpha = speederRenderer.color.a;
    }
    void OnTriggerEnter(Collider other)
    {
        var bounceBall = other.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            targetBody = bounceBall.GetComponent<Rigidbody>();
            speederRenderer.DOKill();
            speederRenderer.DOFade(1, 0.05f);
        }
    }
    void OnTriggerExit(Collider other)
    {
        var bounceBall = other.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            targetBody = null;
            speederRenderer.DOKill();
            speederRenderer.DOFade(initAlpha, 0.4f);
        }
    }
    void FixedUpdate()
    {
        if (targetBody != null)
        {
            targetBody.AddForce(transform.right * force, ForceMode.Acceleration);
        }
    }
}
