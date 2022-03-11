using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOETrap : MonoBehaviour
{

    [SerializeField] DamageTargetMode TargetMode;
    [SerializeField] int damageAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((TargetMode == DamageTargetMode.Player && collision.IsPlayer())
            || (TargetMode == DamageTargetMode.Enemy && collision.IsEnemy()))
            Detonate(TargetMode);
    }

    private void Detonate(DamageTargetMode targetMode)
    {
        ExplosionHandler.Instance.Explode(targetMode, transform.position, damageAmount);
        Destroy(gameObject);
    }
}
