using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodeOnHitBehaviour : OnHitBehaviour
{
    [SerializeField] private DamageTargetMode targetMode;
    [SerializeField] private int damage;
    protected override void Execute(Vector2 relativeVelocity)
    {
        ExplosionHandler.Instance.Explode(targetMode, transform.position, damage);
    }
}
