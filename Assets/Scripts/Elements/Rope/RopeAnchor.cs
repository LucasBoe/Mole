using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RopeAnchor : MonoBehaviour
{
    [SerializeField] private Rigidbody2D rigidbody2D;
    [SerializeField] private GameObject[] ropes;
    [SerializeField] private float smoothForceDifference = 0;
    [SerializeField] private float smoothDistanceDifference = 0;

    IRopeable rope1, rope2;

    private const float minDistance = 0.005f;

    public Rigidbody2D Rigidbody2D => rigidbody2D;

    private void Start()
    {
        rope1 = ropes[0].GetComponent<IRopeable>();
        rope2 = ropes[1].GetComponent<IRopeable>();
    }

    internal RopeSlot ClearSlot(IRopeable ropeable)
    {
        if (rope1 == ropeable)
        {
            rope1 = null;
            return RopeSlot.Slot1;
        }
        else if (rope2 == ropeable)
        {
            rope2 = null;
            return RopeSlot.Slot2;
        }

        return RopeSlot.None;
    }

    public RopeSlot GetEmptySlot()
    {
        if (rope1 == null)
            return RopeSlot.Slot1;
        else if (rope2 == null)
            return RopeSlot.Slot2;

        return RopeSlot.None;
    }

    public void ConnectRopeToSlot(IRopeable rope, RopeSlot ropeSlot)
    {
        if (ropeSlot == RopeSlot.Slot1)
            rope1 = rope;
        else
            rope2 = rope;
    }

    private void LateUpdate()
    {
        float change = 0;

        if (rope1 == null || rope2 == null)
            return;

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

    public enum RopeSlot
    {
        None,
        Slot1,
        Slot2,
    }
}
