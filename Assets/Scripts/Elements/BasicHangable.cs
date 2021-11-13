using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IHangable
{
    Vector2 GetClosestHangablePosition(Vector2 pos, Vector2 dir);
}

public class BasicHangable : MonoBehaviour, IHangable
{
    [SerializeField] Transform point1, point2;
    public Vector2 GetClosestHangablePosition(Vector2 pos, Vector2 dir)
    {
        Vector2 pointToCheckFrom = pos + dir;
        return Util.GetClosestPointOnLineSegment(point1.position, point2.position, pointToCheckFrom);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(point1.position, point2.position);
    }
}
