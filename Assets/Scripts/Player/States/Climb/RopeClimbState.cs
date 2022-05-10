using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeClimbState : PlayerStateBase
{
    PlayerPhysicsModifier.ColliderMode modeBefore;
    internal static bool TryEnter()
    {
        if (PlayerRopeUser.Instance.IsActive)
        {
            PlayerStateMachine.Instance.SetState(new RopeClimbState());
            return true;
        }

        return false;
    }

    public override void Enter()
    {
        base.Enter();
        //PlayerRopeClimbListener.Instance.TrySetState(PlayerRopeClimbListener.States.Climb);
        PlayerRopeUser.Instance.SetPlayerDragActive(false);
        modeBefore = PlayerPhysicsModifier.Instance.Mode;
        PlayerPhysicsModifier.Instance.SetColliderMode(PlayerPhysicsModifier.ColliderMode.Tunnel);
    }

    public override void Update()
    {
        base.Update();

        PlayerRopeUser ropeUser = PlayerRopeUser.Instance;

        if (context.Input.Jump)
            SetState(new JumpState());

        if (ropeUser.IsActive)
        {
            Rigidbody2D body = ropeUser.GetRopeElementBody();
            body.AddForce(context.Input.Axis * Time.deltaTime * context.Values.RopeSwingForceCurve.Evaluate(body.velocity.magnitude));
            body.velocity = body.velocity * (1 + Time.deltaTime);

            //transition to hanging
            HangingState.TryEnter(this, context);

            //transition to pullup
            PullUpState.TryEnter(this, context);
        }
        else
        {
            SetState(new IdleState());
        }

    }

    public override void Exit()
    {
        base.Exit();
        PlayerRopeUser.Instance.DropCurrentRope();
        PlayerRopeUser.Instance.SetPlayerDragActive(true);
        PlayerPhysicsModifier.Instance.SetColliderMode(modeBefore);
    }
}