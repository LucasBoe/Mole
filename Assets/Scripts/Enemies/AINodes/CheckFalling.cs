using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CheckFalling : ActionNode
{
    protected override void OnStart()
    {
        context.rigigbodyController.SetFallmodeActive(true);
    }

    protected override void OnStop()
    {
        context.rigigbodyController.SetFallmodeActive(false);
    }

    protected override State OnUpdate()
    {
        return context.groundCheck.IsGrounded || context.groundCheck.GroundTime < 1f ?  State.Failure : State.Running;
    }
}
