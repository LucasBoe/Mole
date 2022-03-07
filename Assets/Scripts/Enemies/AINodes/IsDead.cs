using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsDead : ActionNode
{
    protected override void OnStart()
    {
        if (context.damageModule.Dead)
            context.rigigbodyController.SetDeadMode(true);
    }

    protected override void OnStop()
    {

    }

    protected override State OnUpdate()
    {
        return context.damageModule.Dead ? State.Running : State.Failure;
    }
}
