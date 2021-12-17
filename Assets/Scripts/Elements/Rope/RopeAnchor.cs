using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeAnchor : MonoBehaviour
{
    [SerializeField] Rope[] ropes;
    float smoothForceDifference = 0;
    float smoothDistanceDifference = 0;

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

        bool improve = Mathf.Abs(smoothDistanceDifference) > Mathf.Abs(smoothDistanceDifference - smoothForceDifference);

        Debug.LogWarning(improve + " => " + Mathf.Abs(distanceDifference) + " > " + Mathf.Abs(distanceDifference - forceDifference));

        float decreasedByDistance = Mathf.Max(1 - smoothDistanceDifference, 0) * smoothForceDifference;
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



        rope1.ChangeRopeLength(decreasedByDistance);
        rope2.ChangeRopeLength(-decreasedByDistance);
    }
}
