using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeNoiseOnHitBehaviour : MonoBehaviour
{
    [SerializeField] float velocityForSound = 1f;
    [SerializeField] float noise;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        float velocity = Math.Abs(collision.relativeVelocity.magnitude);

        if (velocity > velocityForSound && noise > 0f)
            NoiseHandler.Instance.MakeNoise(transform.position, noise);
    }
}
