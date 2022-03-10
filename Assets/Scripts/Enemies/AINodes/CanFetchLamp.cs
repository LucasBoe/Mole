using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CanFetchLamp : ActionNode
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        LampSource closest = LampSource.GetClosest(context.transform.position);
        context.closestPotentialLamp = closest;
        return closest != null ? State.Success : State.Failure;
    }
}
