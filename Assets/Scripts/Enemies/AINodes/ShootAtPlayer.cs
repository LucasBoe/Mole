using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class ShootAtPlayer : ActionNode
{
    public float shootingVelocity;
    protected override void OnStart() { }

    protected override void OnStop() { }

    protected override State OnUpdate()
    {
        if (context.memory.CanSeePlayer)
        {
            context.shootModule.Shoot(context.PlayerPos, shootingVelocity);
        }
        return State.Success;
    }
}
