using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Util
{
    public static Vector2 GetClosestPointOnLineSegment(Vector2 lineStart, Vector2 lineEnd, Vector2 point)
    {
        Vector2 AP = point - lineStart;       //Vector from A to P   
        Vector2 AB = lineEnd - lineStart;       //Vector from A to B  

        float magnitudeAB = AB.sqrMagnitude;     //Magnitude of AB vector (it's length squared)     
        float ABAPproduct = Vector2.Dot(AP, AB);    //The DOT product of a_to_p and a_to_b     
        float distance = ABAPproduct / magnitudeAB; //The normalized "distance" from a to your closest point  

        if (distance < 0)     //Check if point projection is over vectorAB     
        {
            return lineStart;

        }
        else if (distance > 1)
        {
            return lineEnd;
        }
        else
        {
            return lineStart + AB * distance;
        }
    }

    internal static Vector2[] SmoothToCurve(Vector2[] points, float distribution = 0.25f)
    {
        Vector2 start = points[0];
        Vector2 end = points[points.Length - 1];

        List<Vector2> result = new List<Vector2>();

        result.Add(start);

        for (int i = 1; i < points.Length - 1; i++)
        {
            result.Add(Vector2.Lerp(points[i - 1], points[i], i == 1 ? 0.66f : 1 - distribution));
            result.Add(Vector2.Lerp(points[i], points[i + 1], i == (points.Length - 1) ? 0.33f : distribution));
        }

        result.Add(end);

        return result.ToArray();
    }

    public static bool CheckLineOfSight(Vector2 from, Vector2 to, string layer)
    {
        return CheckLineOfSight(from, to, new string[] { layer });
    }

    public static bool CheckLineOfSight(Vector2 from, Vector2 to, string[] layers)
    {
        RaycastHit2D hit = Physics2D.Raycast(from, (to - from).normalized, Vector2.Distance(from, to), LayerMask.GetMask(layers));
        if (hit == true)
        {
            Debug.DrawLine(from, hit.point, Color.yellow);
            Debug.DrawLine(hit.point, to, Color.red);
            DebugDrawCross(hit.point, Color.red, 5);
            return false;
        }
        else
        {
            Debug.DrawLine(from, to, Color.green);
        }

        return true;
    }
    public static void DebugDrawCircle(Vector3 position, Color color, float size, int segmentLength = 15, float lifetime = -1f)
    {
        Vector3 lastPosition = Vector3.zero;

        for (int i = 0; i <= 360; i += segmentLength)
        {
            Vector2 positionXY = new Vector2(Mathf.Sin(i * Mathf.Deg2Rad), Mathf.Cos(i * Mathf.Deg2Rad));
            Vector2 positionxyz = new Vector3(position.x + positionXY.x * size, position.y + positionXY.y * size);

            if (lastPosition != Vector3.zero)
            {
                if (lifetime < 0)
                    Debug.DrawLine(lastPosition, positionxyz, color);
                else
                    Debug.DrawLine(lastPosition, positionxyz, color, lifetime);
            }

            lastPosition = positionxyz;
        }
    }

    internal static void DebugDrawBox(Vector2 pos, Vector2 scale, Color color, float lifetime = -1f)
    {
        if (lifetime < 0)
        {
            Debug.DrawLine(pos + new Vector2(scale.x / 2, scale.y / 2), pos + new Vector2(scale.x / 2, -scale.y / 2), color);
            Debug.DrawLine(pos + new Vector2(-scale.x / 2, scale.y / 2), pos + new Vector2(-scale.x / 2, -scale.y / 2), color);
            Debug.DrawLine(pos + new Vector2(scale.x / 2, -scale.y / 2), pos + new Vector2(-scale.x / 2, -scale.y / 2), color);
            Debug.DrawLine(pos + new Vector2(-scale.x / 2, scale.y / 2), pos + new Vector2(scale.x / 2, scale.y / 2), color);
        }
        else
        {
            Debug.DrawLine(pos + new Vector2(scale.x / 2, scale.y / 2), pos + new Vector2(scale.x / 2, -scale.y / 2), color, lifetime);
            Debug.DrawLine(pos + new Vector2(-scale.x / 2, scale.y / 2), pos + new Vector2(-scale.x / 2, -scale.y / 2), color, lifetime);
            Debug.DrawLine(pos + new Vector2(scale.x / 2, -scale.y / 2), pos + new Vector2(-scale.x / 2, -scale.y / 2), color, lifetime);
            Debug.DrawLine(pos + new Vector2(-scale.x / 2, scale.y / 2), pos + new Vector2(scale.x / 2, scale.y / 2), color, lifetime);
        }
    }

    public static void DebugDrawCross(Vector2 position, Color color, float size, float lifetime = -1f)
    {
        if (lifetime < 0)
        {
            Debug.DrawLine(position + new Vector2(size / 2, size / 2), position + new Vector2(-size / 2, -size / 2), color);
            Debug.DrawLine(position + new Vector2(-size / 2, size / 2), position + new Vector2(size / 2, -size / 2), color);
        }
        else
        {
            Debug.DrawLine(position + new Vector2(size / 2, size / 2), position + new Vector2(-size / 2, -size / 2), color, lifetime);
            Debug.DrawLine(position + new Vector2(-size / 2, size / 2), position + new Vector2(size / 2, -size / 2), color, lifetime);
        }
    }

    public static void GizmoDrawArrowLine(Vector2 fromPos, Vector2 toPos)
    {
        float distance = Vector2.Distance(fromPos, toPos);
        Vector2 forward = (toPos - fromPos).normalized * 0.25f;
        Vector2 up = Vector2.Perpendicular(forward);

        for (int i = 0; i < distance; i++)
        {
            Vector2 arrowPos = Vector2.Lerp(fromPos, toPos, i / distance) + up;
            Gizmos.DrawLine(arrowPos + forward, arrowPos + up);
            Gizmos.DrawLine(arrowPos - forward, arrowPos + forward * 0.5f);
            Gizmos.DrawLine(arrowPos + forward, arrowPos - up);
        }
        //Gizmos.DrawLine(fromPos + up * 0.5f, toPos + up * 0.5f);
    }
    public static void GizmoDrawCollisionCheck(CollisionCheck pcc, Vector2 pos)
    {
        Gizmos.color = pcc.DebugColor;
        Gizmos.DrawWireCube(pos + pcc.Pos, pcc.Size);
    }

    public static float GetLongestAxis(this Vector3 vector3)
    {
        float absX = Mathf.Abs(vector3.x);
        float absY = Mathf.Abs(vector3.y);
        float absZ = Mathf.Abs(vector3.z);

        if (absX > absY && absX > absZ)
        {
            return vector3.x;
        }
        else if (absY > absX && absY > absZ)
        {
            return vector3.y;
        }
        else
        {
            return vector3.z;
        }
    }

    public static Vector2 Clamp(this Vector2 vector2, Vector2 min, Vector2 max)
    {
        return new Vector2(Mathf.Clamp(vector2.x, min.x, max.x), Mathf.Clamp(vector2.y, min.y, max.y));
    }

    public static float GetLongestAxis(this Vector2 vector2)
    {
        float absX = Mathf.Abs(vector2.x);
        float absY = Mathf.Abs(vector2.y);

        if (absX > absY)
        {
            return vector2.x;
        }
        else
        {
            return vector2.y;
        }
    }

    public static Vector2 InvertY(this Vector2 vector2)
    {
        return new Vector2(vector2.x, -vector2.y);
    }

    public static Vector3[] ToVector3Array(this Vector2[] v2s)
    {
        List<Vector3> v3s = new List<Vector3>();
        foreach (Vector2 v2 in v2s)
            v3s.Add(v2);
        return v3s.ToArray();
    }

    public static Quaternion ToRotation(this Vector2 dir)
    {
        float angle = Vector2.Angle(dir.normalized, Vector2.right);
        return Quaternion.Euler(0, 0, angle);
    }

    public static Gradient ToGradient(this Color color)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(new GradientColorKey[] { new GradientColorKey(color, 0) }, new GradientAlphaKey[] { new GradientAlphaKey(color.a, 0) });
        return gradient;
    }

    /// <summary>
    /// maps an angle back into a range between 0 and 360
    /// </summary>
    /// <param name="angleToFix"></param>
    /// <returns></returns>

    public static float FixAngle(float angleToFix)
    {
        return angleToFix % 360;
    }

    public static bool IsPlayer(this Collider2D collision)
    {
        return collision.CompareTag("Player");
    }

    public static T GetComponentInChildrenExcludeOwn<T>(this Component origin)
    {
        T own = origin.GetComponent<T>();

        foreach (T component in origin.GetComponentsInChildren<T>())
        {
            if ((object)component != (object)own)
                return component;
        }

        return own;
    }
}
