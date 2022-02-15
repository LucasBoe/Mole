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
    protected CableElement[] elements = new ChainElement[2];
    public CableElement[] Elements => elements;
    public CableElement One => elements[0];
    public CableElement Two => elements[1];
    public bool IsShortCable => anchors.Count == 0;


    //length & distribution
    protected float totalLength;
    protected SmoothFloat smoothLength1;
    protected SmoothFloat smoothLength2;

    [SerializeField, Range(0,1)] protected float distribution = 0.5f;
    protected float deadLength = 0;

    protected List<RopeLengthChange> lengthChanges = new List<RopeLengthChange>();

    protected void DefineElementsShort(CreateChainElementResult newElement)
    {
        elements[0] = newElement.Instance;
    }


    protected void DefineElementsLong(CreateChainElementResult one, CreateChainElementResult two)
    {
        elements[0] = one.Instance;
        elements[1] = two.Instance;
    }

    public bool IsRigidbodyStart(Rigidbody2D rigidbody2D)
    {
        return One.Rigidbody2DAttachedTo == rigidbody2D;
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
        float v1 = (totalLength - deadLength) * distribution;
        float v2 = (totalLength - deadLength) * (1f - distribution);

        if (smoothLength1 == null) smoothLength1 = new SmoothFloat(v1);
        if (smoothLength2 == null) smoothLength2 = new SmoothFloat(v2);

        smoothLength1.Value = v1;
        smoothLength2.Value = v2;
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
        smoothed = Mathf.Lerp(smoothed, raw, Time.deltaTime / duration);
    }
}
