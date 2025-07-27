using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer chargeSprite;
    private bool isCharged = false;

    void OnCollisionEnter(Collision other)
    {
        var bounceBall = other.collider.GetComponent<BounceBall>();
        if (bounceBall != null)
        {
            if (!isCharged)
            {
                isCharged = true;
                chargeSprite.gameObject.SetActive(true);
            }
            else
            {
                EventHandler.Call_OnCollect(this);
            }
        }
    }
}
