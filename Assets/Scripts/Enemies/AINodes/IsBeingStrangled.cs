using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class IsBeingStrangled : ActionNode
{
    protected override void OnStart()
    {
        if (context.memory.IsBeingStrangled)
            context.itemEquipment.DropItem();
    }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return context.memory.IsBeingStrangled ? State.Running : State.Failure;
    }
}
