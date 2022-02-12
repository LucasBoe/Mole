using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRopeClimbListener : SingletonBehaviour<PlayerRopeClimbListener>
{
    [SerializeField] private SpringJoint2D springJoint2D;
    public enum States
    {
        Idle,
        IdleHoverRope,
        Climb,
    }

    [SerializeField] private States currentState = States.Idle;

    private InputAction startClimbing;

    private void Start()
    {
        startClimbing = new InputAction() { ActionCallback = () => PlayerStateMachine.Instance.SetState(new RopeClimbState()), Input = ControlType.Use, Stage = InputActionStage.WorldObject, Target = transform, Text = "Climb Rope" };
    }

    private void Update()
    {
        if (currentState == States.Climb)
            return;


        Collider2D[] ropeColliders = Physics2D.OverlapBoxAll(transform.position, Vector2.one, 0, LayerMask.GetMask("Rope"));
        TrySetState(ropeColliders.Length > 0 ? States.IdleHoverRope : States.Idle);
    }

    public void TrySetState(States toSet)
    {
        States stateBefore = currentState;
        currentState = toSet;

        if (currentState == States.IdleHoverRope)
            PlayerInputActionRegister.Instance.RegisterInputAction(startClimbing);
        else
            PlayerInputActionRegister.Instance.UnregisterInputAction(startClimbing);

        springJoint2D.enabled = toSet == States.Climb;
    }

    internal void SetClimbingBody(Rigidbody2D body)
    {
        springJoint2D.connectedBody = body;
    }

    internal ColliderDistance2D GetDistanceToBody()
    {
        return springJoint2D.connectedBody.Distance(PlayerColliderModifier.Instance.GetActiveCollider());
    }
}
