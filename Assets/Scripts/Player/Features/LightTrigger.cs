using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightTrigger : MonoBehaviour
{
    public PlayerHidingHandler.HiddenAmount HiddenAmount;
    public static System.Action<LightTrigger> PlayerEnteredTrigger, PlayerExitedTrigger;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            PlayerEnteredTrigger?.Invoke(this);
        else if (collision.IsEnemy())
            EnemyNotifyEnter(collision, true);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            PlayerExitedTrigger?.Invoke(this);
        else if (collision.IsEnemy())
            EnemyNotifyEnter(collision, false);
    }

    private void EnemyNotifyEnter(Collider2D collision, bool enter)
    {
        Debug.Log("Notify Enemy");

        EnemyLightModule lightModule = collision.GetComponent<EnemyLightModule>();
        if (lightModule != null)
        {
            if (enter)
                lightModule.Enter(this);
            else
                lightModule.Exit(this);
        }
    }
}
