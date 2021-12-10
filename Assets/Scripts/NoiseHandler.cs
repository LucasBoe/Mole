using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseHandler : SingletonBehaviour<NoiseHandler>
{
    [SerializeField] GameObject noiseVisualization;
    public Action<Vector2, float> OnNoise;

    internal void MakeNoise(Vector3 position, float volume)
    {
        Util.DebugDrawCross(position, Color.white, 1, 5f);
        Destroy(Instantiate(noiseVisualization, position, Quaternion.identity), 4);
        OnNoise?.Invoke(position, volume);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N))
            MakeNoise(Camera.main.ScreenToWorldPoint(Input.mousePosition), 10f);
    }
}
