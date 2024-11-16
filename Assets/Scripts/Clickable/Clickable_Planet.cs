using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clickable_Planet : Basic_Clickable
{
    public override void OnClick(PlayerController player)
    {
        base.OnClick(player);
        Debug.Log("Planet");
    }
}
