using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHandler : SingletonBehaviour<NoiseHandler>
{
    [SerializeField] ParticleSystem noiseVisualization;
    public Action<Vector2, float> OnNoise;

    internal void MakeNoise(Vector3 position, float volume)
    {
        Util.DebugDrawCross(position, Color.white, 1, 5f);
        ParticleSystem particleSystem = Instantiate(noiseVisualization, position, Quaternion.identity);
        SetNoiseParticleSize(particleSystem, volume * 0.1f);
        Destroy(particleSystem.gameObject, 4);
        OnNoise?.Invoke(position, volume);
    }

    private void SetNoiseParticleSize(ParticleSystem particleSystem, float size)
    {
        ParticleSystemRenderer noiseRenderer = particleSystem.GetComponent<ParticleSystemRenderer>();

        Material material = new Material(noiseRenderer.material);
        material.SetFloat("size", size);
        noiseRenderer.material = material;
        noiseRenderer.transform.localScale = new Vector3(size, size, size);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            MakeNoise(Camera.main.ScreenToWorldPoint(Input.mousePosition), 10f);
    }
}
