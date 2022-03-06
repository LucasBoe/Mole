using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class TunnelState : PlayerStateBase
{
    public Vector2Int MoveDir; 
    public TunnelState(Transform switchTransform)
    {
        context.Rigidbody.MovePosition(switchTransform.position);
    }

    public override void Update()
    {
        Vector2 axis = context.Input.Axis;
        if (axis == Vector2.zero)
            MoveDir = Vector2Int.zero;
        else if (Mathf.Abs(axis.x) > Mathf.Abs(axis.y))
            MoveDir = axis.x >= 0 ? Vector2Int.right : Vector2Int.left;
        else
            MoveDir = axis.y >= 0 ? Vector2Int.up : Vector2Int.down;

        context.Rigidbody.velocity = axis * context.Values.XVelocity.NotSprintValue;
    }
}
