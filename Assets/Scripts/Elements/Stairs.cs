using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Stairs : MonoBehaviour
{
    [SerializeField] public PolygonCollider2D trigger;
    [SerializeField] public Sprite sprite225, sprite45;
    [SerializeField] public Transform otherTarget, spriteHolder;
    [SortingLayer]
    public int SortingLayerID;

    EdgeCollider2D edgeCollider;

    private void Awake()
    {
        GameObject edge = new GameObject("edge_auto");

        edge.transform.parent = transform;
        edge.transform.localPosition = Vector3.zero;

        edgeCollider = edge.AddComponent<EdgeCollider2D>();
        edgeCollider.usedByEffector = true;
        edgeCollider.points = new Vector2[] { Vector2.zero, otherTarget.localPosition };
        edge.AddComponent<PlatformEffector2D>().surfaceArc = 160;
    }

    public float GetLowestPoint()
    {
        return Mathf.Min(transform.position.y, otherTarget.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            StairsUser user = collision.GetComponentInChildren<StairsUser>();
            if (user != null)
                user.RegisterPossibleStairs(this);
        }
    }

    internal void SetActive(bool active)
    {
        edgeCollider.enabled = active;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.IsPlayer())
        {
            StairsUser user = collision.GetComponentInChildren<StairsUser>();
            if (user != null)
                user.UnregisterPossibleStairs(this);
        }
    }

    public Vector2 GetClosestPoint(Vector2 point)
    {
        return Util.FindIntersectionOfLines(transform.position, otherTarget.position, new Vector2(point.x, -1), new Vector2(point.x, 1));
    }
}
