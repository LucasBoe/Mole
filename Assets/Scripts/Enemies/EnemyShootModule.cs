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
        Rigidbody2D projectile = Instantiate(projectilePrefab, origin, Quaternion.identity);
        projectile.velocity = (target - transform.position).normalized * shootingVelocity;
    }
}

public class EnemyShootState : EnemyStateBase
{
    EnemyShootModule shootModule;

    bool shot = false;
    float time = 0;
    Transform target;

    public float ShootingVelocity = 25f;
    public float PreeshootTime = 0.5f;
    public float AftershootTime = 0.5f;

    public EnemyShootState(Transform target)
    {
        this.target = target;
    }

    public override bool TryEnter(EnemyBase enemyBase)
    {
        shootModule = enemyBase.GetModule<EnemyShootModule>();

        if (target != null)
            return true;

        return false;
    }

    public override void Update(EnemyBase enemyBase)
    {
        time += Time.deltaTime;

        if (time > PreeshootTime + AftershootTime)
        {
            shot = false;
            time = 0;
        } else if (!shot && time > PreeshootTime)
        {
            shot = true;
            shootModule.Shoot(target.position, ShootingVelocity);
        }
    }

    public override bool TryExit(EnemyBase enemyBase)
    {
        return false;
    }
}
