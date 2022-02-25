using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RopeClimbState : PlayerStateBase
{
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
    }

    public override void Update()
    {
        base.Update();

        if (context.Input.Jump)
            JumpOff(context.Input.Axis);

        Rigidbody2D body = PlayerRopeUser.Instance.GetRopeElementBody();
        body.AddForce(context.Input.Axis * Time.deltaTime * context.Values.RopeSwingForceCurve.Evaluate(body.velocity.magnitude));
        body.velocity = body.velocity * (1 + Time.deltaTime);

    }

    public override void Exit()
    {
        base.Exit();
        PlayerRopeUser.Instance.DropCurrentRope();
        PlayerRopeUser.Instance.SetPlayerDragActive(true);
    }
}