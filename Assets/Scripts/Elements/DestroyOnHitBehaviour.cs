using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHitBehaviour : MonoBehaviour
{
    [SerializeField] float velocityForDestruction = 10f;
    [SerializeField] ParticleSystem prefab;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float velocity = Math.Abs(collision.relativeVelocity.magnitude);
        Debug.Log(name + " hit with velocity: " + velocity);

        if (velocity > velocityForDestruction)
            DestroyAndReplace();
    }


    private void DestroyAndReplace()
    {
        if (prefab != null)
            EffectHandler.Spawn(new CustomEffect(prefab), transform.position);

        Destroy(gameObject);
    }
}
