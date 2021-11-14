using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewcone : MonoBehaviour
{
    [SerializeField] LineRenderer viewConeLines;
    [SerializeField] PolygonCollider2D polygonCollider2D;

    IEnemy enemy;

    private void OnEnable()
    {
        enemy = GetComponentInParent<IEnemy>();
    }

    internal void UpdateBounds(Vector2 eyePosition, float viewConeDistance, float viewConeHeight)
    {
        Vector2[] points = new Vector2[] { eyePosition + Vector2.right * viewConeDistance + Vector2.up * (viewConeHeight / 2f),
            eyePosition,
            eyePosition + Vector2.right * viewConeDistance + Vector2.down * (viewConeHeight / 2f) };

        polygonCollider2D.points = points;
        viewConeLines.positionCount = points.Length;
        viewConeLines.SetPositions(points.ToVector3Array());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            enemy.PlayerEnteredViewcone(collision);
    }
}