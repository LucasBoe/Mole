using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathVisualizationSpawner : MonoBehaviour
{
    [SerializeField] Transform toGetFlipFrom;
    [SerializeField] GameObject[] toSpawn;
    [SerializeField] Vector2 offset;

    private void Start()
    {
        GetComponent<EnemyBase>().OnEnemyDeath += OnDeath;
    }

    private void OnDeath()
    {
        Vector3 scale = toGetFlipFrom.localScale;

        foreach (GameObject gameObject in toSpawn)
        {
            GameObject instance = Instantiate(gameObject, (Vector2)transform.position + offset, Quaternion.identity);
            instance.transform.localScale = scale;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere((Vector2)transform.position + offset, 0.5f);
    }
}
