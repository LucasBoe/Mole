using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseListener : MonoBehaviour
{
    [SerializeField] float radius = 10f;
    [SerializeField] float volumeThreshold = 0f;

    System.Action<Vector2> OnNoise;
    private void OnNoiseReceive(Vector2 position, float volume)
    {
        if (Vector2.Distance(transform.position, position) > radius)
            return;

        if (!Util.CheckLineOfSight(transform.position, position, "Default"))
            return;

        if (volume < volumeThreshold)
            return;

        Util.DebugDrawCircle(position, Color.green, 1, lifetime: 5f);
        OnNoise?.Invoke(position);
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, radius);
    }

    private void Start()
    {
        NoiseHandler.Instance.OnNoise += OnNoiseReceive;
    }

    private void OnDestroy()
    {
        NoiseHandler.Instance.OnNoise -= OnNoiseReceive;
    }
}
