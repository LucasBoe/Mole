using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.IsPlayer())
        {
            PlayerHealth.Instance.DoDamage(20);
        }

        Destroy(gameObject);
    }
}
