using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideEnemyRagdoll : MonoBehaviour
{
    [SerializeField] Transform hideTransform;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        EnemyRagdoll enemyRagdoll = collision.GetComponent<EnemyRagdoll>();
        if (enemyRagdoll != null)
        {
            enemyRagdoll.Hide(transform.position);
        }
    }
}
