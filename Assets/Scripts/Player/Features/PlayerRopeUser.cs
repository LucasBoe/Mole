using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopeUser : PlayerSingletonBehaviour<PlayerRopeUser>
{
    public enum RopeUserMode
    {
        Free,
        Grap,
    }

    [SerializeField] RopeUserMode ropeUserMode;
    [SerializeField] AnimationCurve distanceToRopeToLengthChangeCurve;
    [SerializeField] float ropeLengthChangeMultiplier = 1f;

    public RopeUserMode Mode
    {
        get => ropeUserMode;
        set
        {
            ropeUserMode = value;
        }
    }

    [SerializeField] Rigidbody2D playerRigidbody2D;

    [ReadOnly, SerializeField] RopeElement currentElement;
    [ReadOnly, SerializeField] Rope current;
    bool playerConstrollsStart = false;
    public bool IsActive => current != null && currentElement != null;

    public Rigidbody2D ConnectToRope(Rope newRope, bool playerIsAtStart)
    {
        current = newRope;
        playerConstrollsStart = playerIsAtStart;
        currentElement = newRope.GetPlayerControlledElement(playerIsAtStart);

        return playerRigidbody2D;
    }

    public void TryConnectCrossbowRopeBolt(RopeFixture ropeFixture)
    {
        if (!IsActive)
            CableHandler.Instance.CreateRope(playerRigidbody2D, ropeFixture.Rigidbody2D);
        else
            CableHandler.Instance.ExtendRope(current ,ropeFixture, playerRigidbody2D);
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
        if (!IsActive)
            return;

        RopeEnd ropeEnd = CableHandler.Instance.CreateRopeEnd(playerRigidbody2D.position);
        HandoverRopeTo(ropeEnd.SetRope(current));
    }

    public Rope HandoverRopeTo(Rigidbody2D newRigidbody)
    {
        if (current == null)
            return null;

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

        if (input.Axis != Vector2.zero)
            current.Elongate(Time.deltaTime * CalculateElongationFactor(input) * ropeLengthChangeMultiplier, distribution: playerConstrollsStart ? 1f : 0f);
    }

    private float CalculateElongationFactor(PlayerInput playerInput)
    {
        Vector2 endPosition = currentElement.GetOtherEndPosition(transform.position);
        Vector2 currentPosition = transform.position;
        Vector2 movePosition = currentPosition + playerInput.Axis;

        return Vector2.Distance(movePosition, endPosition) - Vector2.Distance(currentPosition, endPosition);
    }
}
