using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RopeAnchor : MonoBehaviour
{
    [SerializeField] GameObject[] ropes;
    [SerializeField] private float smoothForceDifference = 0;
    [SerializeField] private float smoothDistanceDifference = 0;

    IRopeable rope1, rope2;

    private const float minDistance = 0.005f;

    private void Start()
    {
        rope1 = ropes[0].GetComponent<IRopeable>();
        rope2 = ropes[1].GetComponent<IRopeable>();
    }

    //string log = "change" + "\n";
    //string log1 = "diff1" + "\n";
    //string log2 = "diff2" + "\n";
    //string log3 = "diffTotal" + "\n";
    //string log4 = "correction" + "\n";

    private void LateUpdate()
    {
        float change = 0;

        if (rope1.HasControl)
            change = ControlSimulation(rope1, rope2);
        else if (rope2.HasControl)
            change = ControlSimulation(rope2, rope1);
        else
            change = EqualSimulation();

        bool rope1DistTooSmall = (rope1.JointDistance + change) < minDistance;
        bool rope2DistTooSmall = (rope2.JointDistance - change) < minDistance;

        if (!rope1DistTooSmall && !rope2DistTooSmall)
        {
            Debug.LogWarning("Change Rope Length : " + change);
            rope1.ChangeRopeLength(change);
            rope2.ChangeRopeLength(-change);
        }
    }

    internal float GetTotalRopeLength()
    {
        return rope1.JointDistance + rope2.JointDistance + rope1.Buffer + rope2.Buffer;
    }

    private float ControlSimulation(IRopeable controller, IRopeable controlled)
    {
        return controller.PullForce * Time.deltaTime;
    }

    private float EqualSimulation()
    {
        float forceDifference = (rope1.PullForce - rope2.PullForce) * Time.deltaTime;

        float distanceDifference = Mathf.Abs(rope1.DistanceDifference + rope1.DistanceDifference);

        smoothForceDifference = Mathf.Lerp(smoothForceDifference, forceDifference, Time.deltaTime);
        smoothDistanceDifference = Mathf.Lerp(smoothDistanceDifference, distanceDifference, Time.deltaTime);

        float decreasedByDistance = Mathf.Max(1 - smoothDistanceDifference, 0) * smoothForceDifference;
        return decreasedByDistance;
    }

    private void OnDrawGizmos()
    {
        Handles.Label(transform.position, rope1.JointDistance.ToString() + " - " + rope2.JointDistance);
    }
}
