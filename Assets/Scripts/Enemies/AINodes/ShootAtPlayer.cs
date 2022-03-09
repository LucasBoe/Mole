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
            Vector2 playerPos = context.PlayerPos;
            context.memory.Forward = Direction2DUtil.FromPositions(context.transform.position, playerPos);
            context.shootModule.Shoot(playerPos, shootingVelocity);
            return State.Success;
        }

        return State.Failure;
    }
}
