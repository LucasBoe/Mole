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
            context.memory.Forward = playerPos.x < context.transform.position.x ? Direction2D.Left : Direction2D.Right;
            context.shootModule.Shoot(playerPos, shootingVelocity);
            return State.Success;
        }

        return State.Failure;
    }
}
