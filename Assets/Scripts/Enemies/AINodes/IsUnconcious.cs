using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsUnconcious : ActionNode
{
    private bool wasUnconscious = false;
    protected override void OnStart()
    {
        wasUnconscious = context.memory.IsUnconcious;
    }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (context.memory.IsUnconcious)
            return State.Running;

        return wasUnconscious ? State.Success : State.Failure;
    }
}
