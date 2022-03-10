using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCombatTargetModule : EnemyModule<EnemyCombatTargetModule>, ICombatTarget
{
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] ParticleSystem unconciousEffect;

    private EnemyRigidbodyControllerModule controller;
    private EnemyDamageModule damageModule;
    private EnemyMemoryModule memoryModule;
    public Rigidbody2D Rigidbody2D => rigidbody2D;
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
        memoryModule.SetStrangled(true);
        return true;
    }

    public void StopStrangling()
    {
        memoryModule.SetStrangled(false);
    }

    private IEnumerator UnconsciousRoutine()
    {
        const int unconciousDuration = 10;
        memoryModule.IsUnconcious = true;
        EffectHandler.Spawn(new CustomEffect(unconciousEffect, unconciousDuration), transform.position);
        yield return new WaitForSeconds(unconciousDuration);
        memoryModule.IsUnconcious = false;
    }

    public void FinishStrangling()
    {
        StopAllCoroutines();
        StartCoroutine(UnconsciousRoutine());
    }
}
