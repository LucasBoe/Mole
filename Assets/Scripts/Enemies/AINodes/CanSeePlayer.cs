using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CanSeePlayer : ActionNode
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (context.memory.CanSeePlayer)
            return State.Success;
        else
            return State.Failure;
    }
}