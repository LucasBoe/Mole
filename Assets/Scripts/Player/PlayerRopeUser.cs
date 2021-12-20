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

    RopeUserMode ropeUserMode;

    RopeUserMode Mode
    {
        get => ropeUserMode;
        set
        {
            ropeUserMode = value;
            currentElement.FixateDistance(ropeUserMode != RopeUserMode.Free);
        }
    }

    [SerializeField] Rigidbody2D playerRigidbody2D;

    RopeElement currentElement;
    Rope current;
    bool playerConstrollsStart = false;
    public bool IsActive => current != null;

    float distBefore;
    float distanceDifference;

    public Rigidbody2D ConnectToRope(Rope newRope, bool playerIsAtStart)
    {
        current = newRope;
        playerConstrollsStart = playerIsAtStart;
        currentElement = playerConstrollsStart ? current.One : current.Two;
        distBefore = 0;
        currentElement.FixateDistance(false);

        return playerRigidbody2D;
    }


    public Rope HandoverRopeTo(Rigidbody2D newRigidbody)
    {
        currentElement.FixateDistance(true);
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

    public void DisconnectFromRope()
    {
        currentElement.FixateDistance(true);
    }

    private void Update()
    {
        if (!IsActive)
            return;

        if (PlayerInputHandler.PlayerInput.LTDown)
            Mode = RopeUserMode.Grap;
        else if (PlayerInputHandler.PlayerInput.LTUp)
            Mode = RopeUserMode.Free;

        float dist = Vector2.Distance(currentElement.Rigidbody2DOther.position, currentElement.Rigidbody2DAttachedTo.position);
        if (distBefore != 0f && Mode == RopeUserMode.Free)
        {
            distanceDifference = dist - distBefore;

                current.Elongate(distanceDifference, distribution: playerConstrollsStart ? 1f : 0f);
        }

        distBefore = dist;
    }

    private void OnDrawGizmos()
    {
        if (distanceDifference > 0)
            Util.GizmoDrawArrowLine(currentElement.Rigidbody2DAttachedTo.position, currentElement.Rigidbody2DOther.position);
        else if (distanceDifference < 0)
            Util.GizmoDrawArrowLine(currentElement.Rigidbody2DOther.position, currentElement.Rigidbody2DAttachedTo.position);
    }
}
