using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class StandUp : ActionNode
{
    protected override void OnStart()
    {
        context.rigigbodyController.StartStandingUp();
    }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (!context.rigigbodyController.TriesStandingUp)
            return State.Failure;

        return context.rigigbodyController.IsStanding ? State.Success : State.Running;
    }
}
