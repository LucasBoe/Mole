using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AOETrap : MonoBehaviour
{
    public enum Mode
    {
        Player,
        Enemy,
    }

    [SerializeField] Mode TargetMode;
    [SerializeField] int damageAmount;
    [SerializeField] AnimationCurve distanceToForceCurve = AnimationCurve.EaseInOut(0, 1000, 5, 0);
    [SerializeField] ParticleSystem explosionEffectPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if ((TargetMode == Mode.Player && collision.IsPlayer())
            || (TargetMode == Mode.Enemy && collision.IsEnemy()))
            Detonate(TargetMode);
    }

    private void Detonate(Mode targetMode)
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 3);

        foreach (Collider2D collider in colliders)
        {
            Rigidbody2D body = collider.attachedRigidbody;
            IHealth target = null;

            if (targetMode == Mode.Player)
            {
                target = collider.GetComponent<PlayerHealth>();
            }
            else if (targetMode == Mode.Enemy)
            {
                target = collider.GetComponent<EnemyDamageModule>();
            }

            if (target != null)
                target.DoDamage(damageAmount);

            if (body != null)
                ApplyExplosionForce(body, transform.position);
        }

        EffectHandler.Spawn(new CustomEffect(explosionEffectPrefab, 5f), transform.position);
        Destroy(gameObject);
    }

    private void ApplyExplosionForce(Rigidbody2D body, Vector2 origin)
    {

        Vector2 vector = body.position - origin;
        body.AddForce(vector.normalized * distanceToForceCurve.Evaluate(vector.magnitude), ForceMode.VelocityChange);
    }
}
