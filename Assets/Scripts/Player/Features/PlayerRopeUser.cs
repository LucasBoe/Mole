using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopeUser : SingletonBehaviour<PlayerRopeUser>
{
    public enum RopeUserMode
    {
        Free,
        Grap,
    }

    [SerializeField] RopeUserMode ropeUserMode;
    [SerializeField] AnimationCurve distanceToRopeToLengthChangeCurve;

    public RopeUserMode Mode
    {
        get => ropeUserMode;
        set
        {
            ropeUserMode = value;
        }
    }

    [SerializeField] Rigidbody2D playerRigidbody2D;

    [SerializeField] RopeElement currentElement;
    [SerializeField] Rope current;
    bool playerConstrollsStart = false;
    public bool IsActive => current != null && currentElement != null;

    float distBefore;
    float distanceDifference;
    public Rigidbody2D ConnectToRope(Rope newRope, bool playerIsAtStart)
    {
        Debug.LogWarning("connect to rope: " + newRope + " / " + playerIsAtStart);

        current = newRope;
        playerConstrollsStart = playerIsAtStart;
        currentElement = newRope.GetPlayerControlledElement(playerIsAtStart);
        distBefore = 0;

        return playerRigidbody2D;
    }

    public Rigidbody2D GetRopeElementBody()
    {
        return currentElement.OwnRigidbody;
    }

    /// <summary>
    /// Creates a new RopeEnd at the players position and hands over the rope.
    /// </summary>
    internal void DropCurrentRope()
    {
        RopeEnd ropeEnd = CableHandler.Instance.CreateRopeEnd(playerRigidbody2D.position);
        HandoverRopeTo(ropeEnd.SetRope(current));
    }

    public Rope HandoverRopeTo(Rigidbody2D newRigidbody)
    {
        current.ReplaceConnectedBody(playerRigidbody2D, newRigidbody);

        Rope r = current;
        current = null;
        return r;
    }

    public void TakeRopeFrom(Rope rope, Rigidbody2D rigidbody2D)
    {
        rope.ReplaceConnectedBody(rigidbody2D, playerRigidbody2D);
        ConnectToRope(rope, rope.IsRigidbodyStart(playerRigidbody2D));
    }

    public void SetPlayerDragActive(bool active)
    {
        playerRigidbody2D.drag = active ? 10f : 0f;
    }

    private void Update()
    {
        if (!IsActive)
            return;

        PlayerInput input = PlayerInputHandler.PlayerInput;

        if (input.LT)
            current.Elongate(-Time.deltaTime * input.LTAxis, distribution: playerConstrollsStart ? 1f : 0f);

        //if (input.LTDown)
        //    Mode = RopeUserMode.Grap;
        //else if (input.LTUp)
        //    Mode = RopeUserMode.Free;
        //
        //
        //float dist = Vector2.Distance(currentElement.Rigidbody2DOther.position, currentElement.Rigidbody2DAttachedTo.position);
        //if (distBefore != 0f && Mode == RopeUserMode.Free && input.Axis != Vector2.zero)
        //{
        //    //distanceDifference = dist - distBefore;
        //    distanceDifference = distanceToRopeToLengthChangeCurve.Evaluate(currentElement.DistanceToAttachedObject);
        //
        //    current.Elongate(distanceDifference * Time.deltaTime, distribution: playerConstrollsStart ? 1f : 0f);
        //
        //}
        //
        //distBefore = dist;
    }

    private void OnDrawGizmos()
    {
        if (currentElement == null)
            return;

        if (distanceDifference > 0)
            Util.GizmoDrawArrowLine(currentElement.Rigidbody2DAttachedTo.position, currentElement.Rigidbody2DOther.position);
        else if (distanceDifference < 0)
            Util.GizmoDrawArrowLine(currentElement.Rigidbody2DOther.position, currentElement.Rigidbody2DAttachedTo.position);
    }
}
