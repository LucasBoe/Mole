using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Puddle : MonoBehaviour
{
    [SerializeField] Transform walkTroughPuddleEffect;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Vector2 impact = collision.attachedRigidbody.velocity;
        float impactStrength = impact.magnitude;

        Debug.Log($"Impact with force of { impactStrength }");

        Vector2 pos = collision.ClosestPoint(transform.position);

        EffectHandler.Spawn(new WaterSplashEffect(impact), pos);
        NoiseHandler.Instance.MakeNoise(pos, impactStrength);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        Vector2 newPosition = collision.ClosestPoint(transform.position) + (Vector2.up * 0.125f);
        walkTroughPuddleEffect.forward = (newPosition - (Vector2)walkTroughPuddleEffect.position).normalized;
        walkTroughPuddleEffect.position = newPosition;
    }
}
