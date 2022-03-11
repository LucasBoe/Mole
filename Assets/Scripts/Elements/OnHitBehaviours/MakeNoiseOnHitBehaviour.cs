using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MakeNoiseOnHitBehaviour : OnHitBehaviour
{
    [SerializeField] float noise;

    protected override void Execute(Vector2 relativeVelocity)
    {
        if (noise > 0f)
            NoiseHandler.Instance.MakeNoise(transform.position, noise);
    }
}
