using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class OnHitBehaviour : MonoBehaviour
{
    [SerializeField] protected float treshhold = 10f;
    float spawnTimestamp = 0;

    private void Awake()
    {
        spawnTimestamp = Time.time;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float velocity = Mathf.Abs(collision.relativeVelocity.magnitude);

        if (velocity > treshhold && (spawnTimestamp + 0.01f) < Time.time)
            Execute(collision.relativeVelocity);
    }

    protected abstract void Execute(Vector2 relativeVelocity);
}
