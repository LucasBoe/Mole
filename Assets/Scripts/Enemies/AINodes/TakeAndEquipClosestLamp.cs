using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class TakeAndEquipClosestLamp : ActionNode
{
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        return context.itemEquipment.TryEquip(context.closestPotentialLamp) ? State.Success : State.Failure;
    }
}
