using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rope : Cable
{
    public enum Modes
    {
        Static,
        Player,
        Loose,
    }

    private Modes mode;

    public Rigidbody2D RigidbodyStart => One.Rigidbody2DAttachedTo;
    public Rigidbody2D RigidbodyEnd => IsShortCable ? One.Rigidbody2DOther : Two.Rigidbody2DAttachedTo;
    public List<CableAnchor> Anchors => anchors;

    //use same constructor as chain
    public Rope(Rigidbody2D start, List<CableAnchor> cableAnchors, Rigidbody2D end) : base(start, cableAnchors, end) { }

    //custom rope contructor for thrown ropes
    public Rope(Rigidbody2D start, Rigidbody2D end, Vector2[] pathPoints) : base(start, end, pathPoints)
    {
        RopeElement ropeElement = CableHandler.Instance.SpawnRopeElement(start, end);
        ropeElement.Setup(start, end, pathPoints);
        DefineElementsShort(new CreateCableElementResult() { Instance = ropeElement, Length = totalLength });
    }

    protected override CableElement CreateElementBetween(Rigidbody2D start, Rigidbody2D end, float length)
    {
        RopeElement instance = CableHandler.Instance.SpawnRopeElement(start, end);
        instance.Setup(start, end, length);
        return instance;
    }


    //public Rope(Rigidbody2D start, CableAnchor[] anchors, Rigidbody2D end, Vector2[] travelPoints)
    //{
    //    this.anchors = new List<CableAnchor>(anchors);
    //    if (IsShortRope)
    //    {
    //        if (travelPoints == null)
    //        {
    //            totalLength = Vector2.Distance(start.position, end.position);
    //            elements[0] = CableHandler.Instance.CreateRopeElement(start, end, totalLength);
    //        }
    //        else
    //        {
    //            totalLength = travelPoints.GetDistance();
    //            elements[0] = CableHandler.Instance.CreateRopeElement(start, end, totalLength, travelPoints);
    //        }
    //        distribution = 0;
    //    }
    //    else
    //    {
    //        RecalulateLengthAndDistributionFromDistance(start, end, bufferLength: 1f);
    //        elements[0] = CableHandler.Instance.CreateRopeElement(start, anchors[0].Rigidbody2D, smoothLength1.Value);
    //        elements[1] = CableHandler.Instance.CreateRopeElement(end, anchors[anchors.Length - 1].Rigidbody2D, smoothLength2.Value);
    //    }
    //
    //}

    public override void Update()
    {
        //work down player length changes
        for (int i = lengthChanges.Count - 1; i >= 0; i--)
        {
            RopeLengthChange lengthChange = lengthChanges[i];
            if (lengthChange != null)
            {
                float lengthBefore = totalLength;
                totalLength = lengthBefore + lengthChange.Amount;
                distribution = RecalculateDitribution(lengthChange, lengthBefore, totalLength);
                lengthChanges.RemoveAt(i);
            }
        }

        base.Update();
    }
    private float RecalculateDitribution(RopeLengthChange lengthChange, float lengthBefore, float newLength)
    {
        return (((lengthBefore - deadLength) * distribution) + (lengthChange.Amount * lengthChange.Distribution)) / (newLength - deadLength);
    }

    internal RopeElement GetPlayerControlledElement(bool playerIsAtStart)
    {
        return (playerIsAtStart ? One : Two) as RopeElement;
    }

    public void ReplaceConnectedBody(Rigidbody2D from, Rigidbody2D to)
    {
        if (IsRigidbodyStart(from))
            One.Reconnect(to);
        else
            Two.Reconnect(to);

        //TODO: Remove this failsave
        if (IsShortCable)
            One.SetJointDistance(totalLength);
    }
}
