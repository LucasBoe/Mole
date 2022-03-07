using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatTargetModule : EnemyModule<EnemyCombatTargetModule>, ICombatTarget
{
    [SerializeField] Rigidbody2D rigidbody2D;
    private EnemyRigidbodyControllerModule controller;
    private EnemyDamageModule damageModule;
    public Rigidbody2D Rigidbody2D => rigidbody2D;
    public Vector2 StranglePosition => Vector2.zero;
    public ICollisionModifier CollisionModifier => controller;
    public bool IsNull => this == null;

    public bool IsAlive => !damageModule.Dead;

    private void Start()
    {
        controller = GetModule<EnemyRigidbodyControllerModule>();
        damageModule = GetModule<EnemyDamageModule>();
    }

    public void Kick(Vector2 vector2)
    {
        controller.Kick(vector2);
    }

    public void Kill()
    {
        GetModule<EnemyDamageModule>().Kill();
    }

    public bool StartStrangling()
    {
        return false;
    }

    public void StopStrangling(Vector2 playerPos)
    {
        //
    }
}
