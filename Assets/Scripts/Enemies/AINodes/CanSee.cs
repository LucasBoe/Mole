using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class CanSee : ActionNode
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return context.lightModule.CanSee || Time.time < 1 ? State.Success : State.Failure;
    }
}
