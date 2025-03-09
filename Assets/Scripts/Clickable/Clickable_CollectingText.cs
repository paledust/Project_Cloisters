using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_CollectingText : Basic_Clickable
{
    [SerializeField] private char collectKey;
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        EventHandler.Call_OnCollectExperimentalText(collectKey);
        gameObject.SetActive(false);
    }
}
