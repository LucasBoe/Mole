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
            currentElement.FixateDistance(ropeUserMode != RopeUserMode.Free);
        }
    }

    [SerializeField] Rigidbody2D playerRigidbody2D;

    RopeElement currentElement;
    Rope current;
    bool playerConstrollsStart = false;
    public bool IsActive => current != null && currentElement != null;

    float distBefore;
    float distanceDifference;

    private InputAction startClimbing;

    private void Start()
    {
        startClimbing = new InputAction() { ActionCallback = () => PlayerStateMachine.Instance.SetState(new RopeClimbState()) , Input = ControlType.Use, Stage = InputActionStage.WorldObject, Target = transform, Text = "Climb Rope" };
    }
    public Rigidbody2D ConnectToRope(Rope newRope, bool playerIsAtStart)
    {
        current = newRope;
        playerConstrollsStart = playerIsAtStart;
        currentElement = playerConstrollsStart ? current.One : current.Two;
        distBefore = 0;
        currentElement.FixateDistance(false);

        PlayerInputActionRegister.Instance.RegisterInputAction(startClimbing);

        return playerRigidbody2D;
    }

    /// <summary>
    /// Creates a new RopeEnd at the players position and hands over the rope.
    /// </summary>
    internal void DropCurrentRope()
    {
        RopeEnd ropeEnd = RopeHandler.Instance.CreateRopeEnd(playerRigidbody2D.position);
        HandoverRopeTo(ropeEnd.SetRope(current));
    }

    public Rope HandoverRopeTo(Rigidbody2D newRigidbody)
    {
        currentElement.FixateDistance(true);
        current.ReplaceConnectedBody(playerRigidbody2D, newRigidbody);

        PlayerInputActionRegister.Instance.UnregisterInputAction(startClimbing);

        Rope r = current;
        current = null;
        return r;
    }

    public void TakeRopeFrom(Rope rope, Rigidbody2D rigidbody2D)
    {
        rope.ReplaceConnectedBody(rigidbody2D, playerRigidbody2D);
        ConnectToRope(rope, rope.IsRigidbodyStart(playerRigidbody2D));
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
            //distanceDifference = dist - distBefore;
            distanceDifference = distanceToRopeToLengthChangeCurve.Evaluate(currentElement.DistanceToAttachedObject);

            current.Elongate(distanceDifference * Time.deltaTime, distribution: playerConstrollsStart ? 1f : 0f);

        }

        distBefore = dist;
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
