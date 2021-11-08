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
}
