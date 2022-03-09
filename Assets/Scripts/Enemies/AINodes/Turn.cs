using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class Turn : ActionNode
{

    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        context.memory.Forward = context.memory.Forward.Inverted();
        return State.Success;
    }
}
