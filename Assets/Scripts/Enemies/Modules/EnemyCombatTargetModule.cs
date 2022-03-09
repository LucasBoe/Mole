using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatTargetModule : EnemyModule<EnemyCombatTargetModule>, ICombatTarget
{
    [SerializeField] Rigidbody2D rigidbody2D;

    private EnemyRigidbodyControllerModule controller;
    private EnemyDamageModule damageModule;
    private EnemyMemoryModule memoryModule;
    public Rigidbody2D Rigidbody2D => rigidbody2D;
    public Vector2 StranglePosition => Vector2.zero;
    public ICollisionModifier CollisionModifier => controller;
    public bool IsNull => this == null;

    public bool IsAlive => !damageModule.Dead;

    public Vector2 Position => transform.position;

    public EnemyMemoryModule Memory => memoryModule;

    private void Start()
    {
        controller = GetModule<EnemyRigidbodyControllerModule>();
        damageModule = GetModule<EnemyDamageModule>();
        memoryModule = GetModule<EnemyMemoryModule>();
        damageModule.DamageTrigger.TriggerEntered += OnTriggerEntered;
    }

    private void OnTriggerEntered(EnemyDamager damager)
    {
        if (damager.DoKnock)
        {
            Vector2 knockVector = damager.FetchVelocity();

            if (knockVector.magnitude > 1)
                Knock(knockVector);
        }
    }

    public void Knock(Vector2 vector2)
    {
        controller.Knock(vector2);
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
