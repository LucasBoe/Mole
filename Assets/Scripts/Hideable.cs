using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Hideable : PlayerAboveInteractable
{
    //PlayerControlPromptUI promptUI;
    protected override void OnPlayerEnter()
    {
        //promptUI = PlayerControlPromptUI.Show(ControlType.Use, transform.position + (Vector3.up));
    }

    protected override void OnPlayerExit()
    {
        //if (promptUI != null) promptUI.Hide();
    }

    internal static Hideable GetClosestFrom(Hideable[] hideables, Vector2 playerPos)
    {
        return hideables.OrderBy(h => Vector2.Distance(h.transform.position, playerPos)).First();
    }
}
