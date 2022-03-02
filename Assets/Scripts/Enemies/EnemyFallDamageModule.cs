using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyFallDamageModule : EnemyModule<EnemyFallDamageModule>
{
    [SerializeField] EnemyGroundCheckModule groundCheckModule;
    [SerializeField] EnemyDamageModule damageModule;
    [SerializeField] AnimationCurve fallDurationToDamageCurve = AnimationCurve.Linear(0, 0, 10, 100);
    [SerializeField, ReadOnly] float currentFallDuration;
    [SerializeField, ReadOnly] bool isFalling;

    private void Start()
    {
        groundCheckModule.LeftGround += () => { isFalling = true;  };
        groundCheckModule.EnteredGround += () => { isFalling = false; HandleFalldamage(currentFallDuration); };
    }

    private void HandleFalldamage(float currentFallDuration)
    {
        damageModule.DoDamage(Mathf.RoundToInt(fallDurationToDamageCurve.Evaluate(currentFallDuration)));
        currentFallDuration = 0f;
    }

    private void FixedUpdate()
    {
        if (isFalling)
            currentFallDuration += Time.fixedDeltaTime;
    }
}
