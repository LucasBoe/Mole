using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    [SerializeField] Rigidbody2D rigidbody2D;
    [SerializeField] private int damageMultiplier = 10;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        EnemyDamageModule enemyDamageModule = collision.gameObject.GetComponent<EnemyDamageModule>();
        if (enemyDamageModule) {
            Vector2 vel = collision.relativeVelocity;
            int damage = (int)(vel.magnitude * damageMultiplier);
            enemyDamageModule.DoDamage(damage);
            rigidbody2D.velocity = vel * 0.5f;
        }
    }
}
