using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DamageTargetMode
{
    Player,
    Enemy,
}

public class ExplosionHandler : SingletonBehaviour<ExplosionHandler>
{
    [SerializeField] AnimationCurve distanceToForceCurve = AnimationCurve.EaseInOut(0, 1000, 5, 0);
    [SerializeField] ParticleSystem explosionEffectPrefab;

    public void Explode(DamageTargetMode mode, Vector2 position, int damageAmount)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(position, 3);

        foreach (Collider2D collider in colliders)
        {
            Rigidbody2D body = collider.attachedRigidbody;
            IHealth target = null;

            if (mode == DamageTargetMode.Player)
            {
                target = collider.GetComponent<PlayerHealth>();
            }
            else if (mode == DamageTargetMode.Enemy)
            {
                target = collider.GetComponent<EnemyDamageModule>();
            }

            if (target != null)
                target.DoDamage(damageAmount);

            if (body != null)
                ApplyExplosionForce(body, position);
        }

        EffectHandler.Spawn(new CustomEffect(explosionEffectPrefab, 5f), transform.position);
        NoiseHandler.Instance.MakeNoise(transform.position, 10);
    }

    private void ApplyExplosionForce(Rigidbody2D body, Vector2 origin)
    {
        Vector2 vector = body.position - origin;
        body.AddForce(vector.normalized * distanceToForceCurve.Evaluate(vector.magnitude), ForceMode.VelocityChange);
    }
}
