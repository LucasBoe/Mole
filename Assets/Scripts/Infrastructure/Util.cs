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

        if (distance < 0)     //Check if P projection is over vectorAB     
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

    internal static void GizmoDrawArrowLine(Vector2 fromPos, Vector2 toPos)
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

    public static Vector3[] ToVector3Array(this Vector2[] v2s)
    {
        List<Vector3> v3s = new List<Vector3>();
        foreach (Vector2 v2 in v2s)
            v3s.Add(v2);
        return v3s.ToArray();
    }

    public static bool IsPlayer(this Collider2D collision)
    {
        return collision.CompareTag("Player");
    }
}
