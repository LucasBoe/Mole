using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsAlerted : ActionNode
{
    float enterTime;
    protected override void OnStart()
    {
        enterTime = Time.time;
    }

    protected override void OnStop()
    {
        context.memory.ReactedToAlert = true;
    }

    protected override State OnUpdate()
    {
        if (context.memory.IsAlerted)
        {
            if (context.memory.ReactedToAlert || enterTime + 1 < Time.time)
                return State.Success;

            return State.Running;
        }
        else
            return State.Failure;
    }
}
