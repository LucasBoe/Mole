using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyObervationAreaTrigger : PlayerTriggerBehaviour
{
    public System.Action<Rigidbody2D> PlayerEnter, PlayerExit;
    public bool PlayerIsInside = false;

    protected override void OnPlayerEnter2D(Collider2D playerCollider)
    {
        PlayerEnter?.Invoke(playerCollider.attachedRigidbody);
        PlayerIsInside = true;
    }

    protected override void OnPlayerExit2D(Collider2D playerCollider)
    {
        PlayerExit?.Invoke(playerCollider.attachedRigidbody);
        PlayerIsInside = false;
    }

    internal static bool IsPlayerInside(List<EnemyObervationAreaTrigger> obervationAreas)
    {
        if (obervationAreas == null || obervationAreas.Count < 1) return false;

        foreach (EnemyObervationAreaTrigger obervationArea in obervationAreas)
        {
            if (obervationArea.PlayerIsInside)
                return true;
        }

        return false;
    }
}
