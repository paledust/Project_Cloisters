using System.Collections;
using System.Collections.Generic;
using SimpleAudioSystem;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    [SerializeField] private SpriteRenderer chargeSprite;
    [SerializeField] private AudioData_SO sfxCollect;
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
                AudioManager.Instance.PlaySFX(sfxCollect.AudioKey, 1);
                EventHandler.Call_OnCollect(this);
            }
        }
    }
}
