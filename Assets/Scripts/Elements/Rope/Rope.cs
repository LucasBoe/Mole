using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Rope
{
    //anchors
    private List<RopeAnchor> anchors;

    //elememts
    private RopeElement[] elements = new RopeElement[2];
    public RopeElement[] Elements => elements;
    public RopeElement One => elements[0];
    public RopeElement Two => elements[1];
    public bool IsShortRope => anchors.Count == 0;


    //length & distribution
    private float length;
    private float distribution = 0.5f;
    private float deadLength = 0;

    //smoothing
    private float smoothForceDifference = 0;

    List<RopeLengthChange> lengthChanges = new List<RopeLengthChange>();

    public Rope(Rigidbody2D start, RopeAnchor[] anchors, Rigidbody2D end)
    {
        this.anchors = new List<RopeAnchor>(anchors);
        if (IsShortRope)
        {
            elements[0] = RopeHandler.Instance.CreateRopeElement(start, end);
            length = Vector2.Distance(start.position, end.position);
            distribution = 0;
        }
        else
        {
            elements[0] = RopeHandler.Instance.CreateRopeElement(start, anchors[0].Rigidbody2D);
            elements[1] = RopeHandler.Instance.CreateRopeElement(end, anchors[anchors.Length - 1].Rigidbody2D);
            RecalulateLengthAndDistributionFromDistance();
        }

    }

    public void ReplaceConnectedBody(Rigidbody2D from, Rigidbody2D to)
    {
        if (IsRigidbodyStart(from))
            One.Reconnect(to);
        else
            Two.Reconnect(to);

        //TODO: Remove this failsave
        if (IsShortRope)
            One.SetJointDistance(length);
    }

    public bool IsRigidbodyStart(Rigidbody2D rigidbody2D)
    {
        Debug.LogWarning($" is {rigidbody2D} actually {One.Rigidbody2DAttachedTo}? : {One.Rigidbody2DAttachedTo == rigidbody2D}");
        return One.Rigidbody2DAttachedTo == rigidbody2D;
    }

    public void Update()
    {
        //work down player length changes
        for (int i = lengthChanges.Count - 1; i >= 0; i--)
        {
            RopeLengthChange lengthChange = lengthChanges[i];
            if (lengthChange != null)
            {
                float lengthBefore = length;
                length = lengthBefore + lengthChange.Amount;
                distribution = RecalculateDitribution(lengthChange, lengthBefore, length);
                lengthChanges.RemoveAt(i);
            }
        }

        Debug.LogWarning("new length: " + length + " (is short? " + IsShortRope +  " )");

        if (!IsShortRope)
        {
            ////balance
            float distributionChange = (BalanceOperationn() / length);
            distribution += distributionChange;
            distribution = Mathf.Clamp(distribution, 0, 1);

            //new distance
            One.SetJointDistance((length - deadLength) * distribution);
            Two.SetJointDistance((length - deadLength) * (1f - distribution));

            //update bodies
            One.Rigidbody2DAttachedTo.AddForce(Vector2.up);
            Two.Rigidbody2DAttachedTo.AddForce(Vector2.up);
        }
        //TODO: Rewrite this. Had to make mode public and doing this in update is rediculous;
        else // if (PlayerRopeUser.Instance.Mode == PlayerRopeUser.RopeUserMode.Grap)
        {
            //override length in case the player is pulling
            One.SetJointDistance(length);
        }

        float RecalculateDitribution(RopeLengthChange lengthChange, float lengthBefore, float newLength)
        {
            return (((lengthBefore - deadLength) * distribution) + (lengthChange.Amount * lengthChange.Distribution)) / (newLength - deadLength);
        }
    }

    private float BalanceOperationn()
    {
        float forceDifference = (One.PullForce - Two.PullForce) * Time.deltaTime;
        smoothForceDifference = Mathf.Lerp(smoothForceDifference, forceDifference, Time.deltaTime);
        return smoothForceDifference;
    }

    private void RecalulateLengthAndDistributionFromDistance()
    {
        Vector2[] points = GetPointsFromRigidbodys();

        float startLength = 0;
        float endLength = 0;
        deadLength = 0;

        for (int i = 1; i < points.Length; i++)
        {
            float distance = Vector2.Distance(points[i - 1], points[i]);
            if (i == 1)
                startLength = distance;
            else if (i == points.Length - 1)
                endLength = distance;
            else
                deadLength += distance;
        }

        distribution = startLength / (startLength + endLength);
        length = startLength + deadLength + endLength;
    }

    private Vector2[] GetPointsFromRigidbodys()
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(elements[0].Rigidbody2DAttachedTo.position);
        points.AddRange(anchors.Select(a => a.Rigidbody2D.position));
        points.Add(elements[1].Rigidbody2DAttachedTo.position);

        return points.ToArray();
    }

    public void Elongate(float amount, float distribution)
    {
        lengthChanges.Add(new RopeLengthChange(distribution, amount));
    }
}

public class RopeLengthChange
{
    public float Distribution;
    public float Amount;

    public RopeLengthChange(float distribution, float amount)
    {
        Distribution = distribution;
        Amount = amount;
    }
}
