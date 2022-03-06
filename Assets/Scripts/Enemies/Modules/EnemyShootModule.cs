using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootModule : EnemyModule<EnemyShootModule>
{
    [SerializeField] Rigidbody2D projectilePrefab;
    public void Shoot(Vector3 target, float shootingVelocity)
    {
        Vector3 origin = Vector3.MoveTowards(transform.position, target, 1);
        Rigidbody2D projectile = Instantiate(projectilePrefab, origin, Quaternion.identity, LayerHandler.Parent);
        projectile.velocity = (target - transform.position).normalized * shootingVelocity;
    }
}
