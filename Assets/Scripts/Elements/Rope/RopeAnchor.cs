using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnchor : MonoBehaviour
{
    [SerializeField] Rope[] ropes;
    private float smoothForceDifference = 0;
    private float smoothDistanceDifference = 0;

    private const float minDistance = 0.005f;

    //string log = "change" + "\n";
    //string log1 = "diff1" + "\n";
    //string log2 = "diff2" + "\n";
    //string log3 = "diffTotal" + "\n";
    //string log4 = "correction" + "\n";

    private void LateUpdate()
    {
        Rope rope1 = ropes[0], rope2 = ropes[1];

        float forceDifference = (rope1.PullForce - rope2.PullForce) * Time.deltaTime;
        float distanceDifference = Mathf.Abs((rope1.JointDistance - rope1.RealDistance) + (rope2.JointDistance - rope2.RealDistance));

        smoothForceDifference = Mathf.Lerp(smoothForceDifference, forceDifference, Time.deltaTime);
        smoothDistanceDifference = Mathf.Lerp(smoothDistanceDifference, distanceDifference, Time.deltaTime);

        float decreasedByDistance = Mathf.Max(1 - smoothDistanceDifference, 0) * smoothForceDifference;

        //float newDistanceDifference = Mathf.Abs(((rope1.JointDistance + decreasedByDistance) - rope1.RealDistance) + ((rope2.JointDistance - decreasedByDistance) - rope2.RealDistance));
        //bool improve = Mathf.Abs(newDistanceDifference) <= Mathf.Abs(distanceDifference);
        //Debug.LogWarning(improve + (Mathf.Abs(newDistanceDifference) - Mathf.Abs(distanceDifference)).ToString());

        //float fix = Mathf.Lerp(decreasedByDistance, smoothForceDifference, improve ? 0 : 0);

        //log += smoothForceDifference + "\n";
        //log1 += rope1.JointDistance - rope1.RealDistance + "\n";
        //log2 += rope2.JointDistance - rope2.RealDistance + "\n";
        //log3 += smoothDistanceDifference + "\n";
        //log4 += correction + "\n";
        //
        //
        //Debug.LogWarning(log);
        //Debug.LogWarning(log3);
        //Debug.LogWarning(log4);

        bool rope1DistTooSmall = (rope1.JointDistance + decreasedByDistance) < minDistance;
        bool rope2DistTooSmall = (rope2.JointDistance - decreasedByDistance) < minDistance;

        if (!rope1DistTooSmall && !rope2DistTooSmall)
        {
            rope1.ChangeRopeLength(decreasedByDistance);
            rope2.ChangeRopeLength(-decreasedByDistance);
        }
    }
}
