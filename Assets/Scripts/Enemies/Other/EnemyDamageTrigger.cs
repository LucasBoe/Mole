using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamageTrigger : MonoBehaviour
{
    public System.Action<EnemyDamager> TriggerEntered;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyDamager damager = collision.GetComponentInChildren<EnemyDamager>();
        if (damager != null)
            TriggerEntered?.Invoke(damager);

    }
}
