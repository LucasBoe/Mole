using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnHitBehaviour : MonoBehaviour
{
    [SerializeField] float velocityForDestruction = 10f;
    [SerializeField] float velocityForSound = 1f;
    [SerializeField] GameObject prefab;
    [SerializeField] float noise;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float velocity = Math.Abs(collision.relativeVelocity.magnitude);
        Debug.Log(name + " hit with velocity: " + velocity);

        if (velocity > velocityForSound && noise > 0f)
            NoiseHandler.Instance.MakeNoise(transform.position, noise);

        if (velocity > velocityForDestruction)
            DestroyAndReplace();
    }


    private void DestroyAndReplace()
    {
        if (prefab != null)
            Instantiate(prefab, transform.position, Quaternion.identity);

        Destroy(gameObject);
    }
}
