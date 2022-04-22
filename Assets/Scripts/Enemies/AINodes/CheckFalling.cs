using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;
using System;

public class CheckFalling : ActionNode
{
    protected override void OnStart()
    {

        if (!context.groundCheck.IsGrounded && !context.rigigbodyController.IsFallmodeActive)
        {
            context.itemEquipment.DropItem();
            context.ragdollModule.StartRagdolling();
            //context.rigigbodyController.SetFallmodeActive(true);
        }
    }



    protected override void OnStop()
    {
        //if (context.rigigbodyController.IsFallmodeActive)
        //    context.rigigbodyController.SetFallmodeActive(false);
    }

    protected override State OnUpdate()
    {
        if (!context.groundCheck.IsGrounded)
        {
            return State.Running;
        }
        else
        {
            return (context.rigigbodyController.IsStanding) ? State.Failure : State.Success;
        }
    }
}
