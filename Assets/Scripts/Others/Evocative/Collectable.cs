using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer chargeSprite;
    private bool isCharged = false;
    // void OnTriggerEnter(Collider other)
    // {
    //     var bounceBall = other.GetComponent<BounceBall>();
    //     if (bounceBall != null)
    //     {
    //         EventHandler.Call_OnCollect(this);
    //     }
    // }
    void OnCollisionEnter(Collision other)
    {
        var bounceBall = other.collider.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            if (!isCharged)
            {
                isCharged = true;
                chargeSprite.enabled = true;
            }
            else
            {
                EventHandler.Call_OnCollect(this);
            }
        }
    }
}
