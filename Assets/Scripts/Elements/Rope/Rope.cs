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
    private SmoothFloat smoothLength1;
    private SmoothFloat smoothLength2;

    [SerializeField, Range(0,1)] private float distribution = 0.5f;
    private float deadLength = 0;

    //smoothing
    [SerializeField, Range(-5f, 5f)] private float smoothForceDifference = 0;
    [SerializeField, Range(-100f, 100f)] private float forceDifferenceDebug = 0;

    [SerializeField] private float forceSmoothDuration = 0.1f;
    [SerializeField] private float durationChangeMultiplier = 0.5f;

    List<RopeLengthChange> lengthChanges = new List<RopeLengthChange>();

    public Rope(Rigidbody2D start, RopeAnchor[] anchors, Rigidbody2D end, Vector2[] travelPoints)
    {
        this.anchors = new List<RopeAnchor>(anchors);
        if (IsShortRope)
        {
            if (travelPoints == null)
            {
                length = Vector2.Distance(start.position, end.position);
                elements[0] = RopeHandler.Instance.CreateRopeElement(start, end, length);
            }
            else
            {
                length = travelPoints.GetDistance();
                elements[0] = RopeHandler.Instance.CreateRopeElement(start, end, length, travelPoints);
            }
            distribution = 0;
        }
        else
        {
            RecalulateLengthAndDistributionFromDistance(start, end, bufferLength: 1f);
            elements[0] = RopeHandler.Instance.CreateRopeElement(start, anchors[0].Rigidbody2D, smoothLength1.Value);
            elements[1] = RopeHandler.Instance.CreateRopeElement(end, anchors[anchors.Length - 1].Rigidbody2D, smoothLength2.Value);
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

        if (!IsShortRope)
        {
            ////balance
            float distributionChange = (Mathf.Sign(BalanceOperationn()) * Time.deltaTime * durationChangeMultiplier) / length;
            distribution += distributionChange;
            distribution = Mathf.Clamp(distribution, 0, 1);

            UpdateLength();

            smoothLength1.Smooth();
            smoothLength2.Smooth();

            Debug.LogWarning("r1: " + smoothLength1.Value + " \n r2: " + smoothLength2.Value);

            //new distance
            One.SetJointDistance(smoothLength1.Value);
            Two.SetJointDistance(smoothLength2.Value);

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
        float forceDifference = (One.PullForce - Two.PullForce);
        forceDifferenceDebug = forceDifference;
        smoothForceDifference = Mathf.Lerp(smoothForceDifference, forceDifference, Time.deltaTime / forceSmoothDuration);
        return smoothForceDifference;
    }

    private void UpdateLength()
    {
        float v1 = (length - deadLength) * distribution;
        float v2 = (length - deadLength) * (1f - distribution);

        if (smoothLength1 == null) smoothLength1 = new SmoothFloat(v1);
        if (smoothLength2 == null) smoothLength2 = new SmoothFloat(v2);

        smoothLength1.Value = v1;
        smoothLength2.Value = v2;
    }

    private void RecalulateLengthAndDistributionFromDistance(Rigidbody2D start, Rigidbody2D end, float bufferLength)
    {
        Vector2[] points = GetPointsFromRigidbodys(start, end);

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
        length = startLength + deadLength + endLength + bufferLength;
        UpdateLength();
    }

    private Vector2[] GetPointsFromRigidbodys(Rigidbody2D start, Rigidbody2D end)
    {
        List<Vector2> points = new List<Vector2>();
        points.Add(start.position);
        points.AddRange(anchors.Select(a => a.Rigidbody2D.position));
        points.Add(end.position);

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

public class SmoothFloat
{
    private float raw;
    private float smoothed;
    private float v1;

    public SmoothFloat(float startValue)
    {
        this.smoothed = startValue;
    }

    public float Value
    {
        get => smoothed;
        set
        {
            raw = value;
        }
    }

    public void Smooth(float duration = 1)
    {
        Debug.Log("smooth" + raw + " to " + smoothed);

        smoothed = Mathf.Lerp(smoothed, raw, Time.deltaTime / duration);
    }
}
