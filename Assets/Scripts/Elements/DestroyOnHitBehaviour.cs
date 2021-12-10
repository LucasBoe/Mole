using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHitBehaviour : MonoBehaviour
{
    [SerializeField] float velocityForDestruction = 10f;
    [SerializeField] GameObject prefab;

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
            Destroy(Instantiate(prefab, transform.position, Quaternion.identity), 10);

        Destroy(gameObject);
    }
}
