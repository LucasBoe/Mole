using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class Cable
{
    //anchors
    protected List<CableAnchor> anchors;

    //elememts
    protected CableElement[] elements = new CableElement[2];
    public CableElement[] Elements => elements;
    public CableElement One => elements[0];
    public CableElement Two => elements[1];
    public bool IsShortCable => anchors.Count == 0;


    //length & distribution
    [ReadOnly, SerializeField] protected float totalLength;
    protected SmoothFloat smoothLength1;
    protected SmoothFloat smoothLength2;

    [SerializeField, Range(0, 1)] protected float distribution = 0.5f;
    [SerializeField] protected float deadLength = 0;
    [SerializeField] bool debugTEMP = false;

    public float RealLength => totalLength - deadLength;

    protected List<RopeLengthChange> lengthChanges = new List<RopeLengthChange>();

    public Cable(Rigidbody2D start, List<CableAnchor> cableAnchors, Rigidbody2D end)
    {
        anchors = cableAnchors == null ? new List<CableAnchor>() : cableAnchors;
        CalculateLengthAndDistribution(start, anchors, end);

        if (IsShortCable)
        {
            elements[0] = CreateElementBetween(start, end, totalLength);
        }
        else
        {
            elements[0] = CreateElementBetween(start, cableAnchors[0].Rigidbody2D, smoothLength1.Value);
            elements[1] = CreateElementBetween(end, cableAnchors.Last().Rigidbody2D, smoothLength2.Value);
        }
    }

    public Cable(Rigidbody2D start, Rigidbody2D end, Vector2[] pathPoints)
    {
        anchors = new List<CableAnchor>();
        CalculateLengthAndDistribution(start, new List<CableAnchor>(), end, pathPoints);
    }

    private void CalculateLengthAndDistribution(Rigidbody2D start, List<CableAnchor> cableAnchors, Rigidbody2D end, Vector2[] pathPoints = null)
    {
        float length1;
        float length2;
        float lengthDead;

        //predefined path
        if (pathPoints != null)
        {
            length1 = pathPoints.GetDistance();
            length2 = 0;
            lengthDead = 0;
        }

        //shortcable
        else if (cableAnchors.Count == 0)
        {
            length1 = Vector2.Distance(start.position, end.position);
            length2 = 0;
            lengthDead = 0;
        }

        //longcable
        else
        {
            length1 = Vector2.Distance(start.position, cableAnchors[0].Rigidbody2D.position);
            length2 = Vector2.Distance(end.position, cableAnchors.Last().Rigidbody2D.position);
            lengthDead = cableAnchors.Select(c => c.Rigidbody2D.position).ToArray().GetDistance();
        }

        smoothLength1 = new SmoothFloat(length1);
        smoothLength2 = new SmoothFloat(length2);
        deadLength = lengthDead;

        totalLength = length1 + length2 + lengthDead;
        distribution = length1 / (length1 + length2);

        if (debugTEMP)
            Debug.LogWarning($"created cable: l: { totalLength } and d: {distribution} ");
    }

    protected virtual CableElement CreateElementBetween(Rigidbody2D start, Rigidbody2D end, float length)
    {
        return null;
    }

    protected void DefineElementsShort(CreateCableElementResult newElement)
    {
        elements[0] = newElement.Instance;
    }


    protected void DefineElementsLong(CreateCableElementResult one, CreateCableElementResult two)
    {
        elements[0] = one.Instance;
        elements[1] = two.Instance;
    }

    public bool IsRigidbodyStart(Rigidbody2D rigidbody2D)
    {
        return One.Rigidbody2DAttachedTo == rigidbody2D;
    }

    public bool IsRigidbodyEnd(Rigidbody2D rigidbody2D)
    {
        return Two.Rigidbody2DAttachedTo == rigidbody2D;
    }

    public virtual void Update()
    {
        if (!IsShortCable)
        {
            UpdateLength();

            smoothLength1.Smooth();
            smoothLength2.Smooth();

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
            One.SetJointDistance(totalLength);
        }
    }

    private void UpdateLength()
    {
        float updatedLength1 = (totalLength - deadLength) * distribution;
        float updatedLength2 = (totalLength - deadLength) * (1f - distribution);

        if (debugTEMP)
            Debug.LogWarning($"updated l1:{updatedLength1} and l2:{updatedLength2} by d:{distribution}");

        smoothLength1.Value = updatedLength1;
        smoothLength2.Value = updatedLength2;
    }

    /*
    protected void RecalulateLengthAndDistributionFromDistance(Rigidbody2D start, Rigidbody2D end, float bufferLength)
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
        totalLength = startLength + deadLength + endLength + bufferLength;
        UpdateLength();
    }
    */

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

    public SmoothFloat(float startValue)
    {
        this.smoothed = startValue;
        this.raw = startValue;
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
        smoothed = Mathf.Lerp(smoothed, raw, Time.deltaTime / duration);
    }
}
