using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HangableFloor : MonoBehaviour, IHangable
{
    [SerializeField] SpriteRenderer spriteRenderer;

    Vector2Couple points;

    private void Start()
    {
        points = GetEndpoints();
    }

    private Vector2Couple GetEndpoints ()
    {
        var pos = new Vector2(transform.position.x, transform.position.y + 0.25f);
        float width = spriteRenderer.size.x / 2;
        return new Vector2Couple() { a = new Vector2(pos.x + width, pos.y), b = new Vector2 ( pos.x - width, pos.y) };
    }

    private void OnDrawGizmos()
    {
        Vector2Couple couple = GetEndpoints();
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(couple.a, Vector2.one * 0.25f);
        Gizmos.DrawCube(couple.b, Vector2.one * 0.25f);
        Gizmos.DrawLine(couple.a, couple.b);
    }

    public Vector2 GetClosestHangablePosition(Vector2 pos, Vector2 dir)
    {
        return Util.GetClosestPointOnLineSegment(points.a, points.b, pos + dir);
    }

    public Vector2 GetNormalVector()
    {
        return Vector2.up;
    }
}

[System.Serializable]
public class Vector2Couple
{
    public Vector2 a, b;
}
