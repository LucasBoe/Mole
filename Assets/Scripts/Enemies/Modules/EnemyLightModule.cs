using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLightModule : MonoBehaviour
{
    [SerializeField, ReadOnly] List<LightTrigger> activeTriggers = new List<LightTrigger>();
    public bool CanSee => activeTriggers.Count > 0;

    internal void Enter(LightTrigger lightTrigger)
    {
        if (lightTrigger.HiddenAmount == PlayerHidingHandler.HiddenAmount.Visible)
            activeTriggers.AddUnique(lightTrigger);
    }

    internal void Exit(LightTrigger lightTrigger)
    {
        activeTriggers.Remove(lightTrigger);
    }

    private void OnDrawGizmos()
    {
            Gizmos.color = Color.yellow;
        if (CanSee)
        {
            Gizmos.DrawSphere(transform.position + Vector3.up * 2, 0.5f);
        }
        else
        {
            Gizmos.DrawWireSphere(transform.position + Vector3.up * 2, 0.5f);
    
        }
    }
}
