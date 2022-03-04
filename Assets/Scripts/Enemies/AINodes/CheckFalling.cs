using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckFalling : ActionNode
{
    protected override void OnStart()
    {
        if (!IsGrounded())
            context.rigigbodyController.SetFallmodeActive(true);
    }

    protected override void OnStop()
    {
        context.rigigbodyController.SetFallmodeActive(false);
    }

    protected override State OnUpdate()
    {
        return IsGrounded() ? State.Failure : State.Running;
    }

    private bool IsGrounded()
    {
        return context.groundCheck.IsGrounded && context.groundCheck.GroundTime > 1f;
    }
}
