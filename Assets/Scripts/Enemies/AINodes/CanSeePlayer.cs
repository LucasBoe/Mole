using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CanSeePlayer : Wait
{

    protected override State OnUpdate()
    {
        if (context.memory.CanSeePlayer)
            return base.OnUpdate();
        else
            return State.Failure;
    }
}
