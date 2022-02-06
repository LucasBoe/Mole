using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class TunnelState : PlayerStateBase
{
    public TunnelState(Transform switchTransform)
    {
        context.Rigidbody.MovePosition(switchTransform.position);
    }

    public override void Update()
    {
        context.Rigidbody.velocity = Vector2.right * context.Input.Axis.x * context.Values.XVelocity.NotSprintValue;
    }
}
