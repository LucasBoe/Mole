using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyViewcone : MonoBehaviour
{
    [SerializeField] LineRenderer viewConeLines;
    [SerializeField] PolygonCollider2D polygonCollider2D;

    public System.Action<Vector2> OnPlayerEntered;


    internal void UpdateBounds(Vector2 eyePosition, float viewConeDistance, float viewConeHeight)
    {
        transform.localPosition = eyePosition;

        Vector2[] points = new Vector2[] { Vector2.right * viewConeDistance + Vector2.up * (viewConeHeight / 2f),
            Vector2.zero,
            Vector2.right * viewConeDistance + Vector2.down * (viewConeHeight / 2f) };

        polygonCollider2D.points = points;
        viewConeLines.positionCount = points.Length;
        viewConeLines.SetPositions(points.ToVector3Array());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
            OnPlayerEntered?.Invoke(collision.transform.position);
    }
}
