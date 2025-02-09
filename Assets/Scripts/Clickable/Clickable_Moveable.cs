using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_Moveable : Basic_Clickable
{
    public override void OnClick(PlayerController player, Vector3 hitPos)
    {
        base.OnClick(player, hitPos);
        player.HoldInteractable(this);
    }
}
