using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NoiseListener))]
public class EnemyNoiseModule : EnemyModule<EnemyNoiseModule>
{
    EnemyMemoryModule memoryModule;
    NoiseListener noiseListener;
    protected override void Awake()
    {
        base.Awake();
        noiseListener = GetComponent<NoiseListener>();
    }

    private void Start()
    {
        memoryModule = GetModule<EnemyMemoryModule>();
    }

    private void OnEnable() { noiseListener.OnNoise += OnNoise; }
    private void OnDisable() { noiseListener.OnNoise -= OnNoise; }
    private void OnNoise(Vector2 noiseLocation)
    {
        if (!memoryModule.CanSeePlayer)
        {
            memoryModule.PlayerPos = noiseLocation;
            memoryModule.IsAlerted = true;
        }
    }
}
