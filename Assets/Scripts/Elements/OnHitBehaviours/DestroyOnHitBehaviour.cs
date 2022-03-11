using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHitBehaviour : OnHitBehaviour
{
    [SerializeField] ParticleSystem prefab;

    protected override void Execute(Vector2 relativeVelocity)
    {
        if (prefab != null)
            EffectHandler.Spawn(new CustomEffect(prefab), transform.position);

        Destroy(gameObject);
    }
}
