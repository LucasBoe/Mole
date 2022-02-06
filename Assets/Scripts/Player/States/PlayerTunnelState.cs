using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class TunnelState : PlayerStateBase
{
    public static System.Action<bool> OnSetTunnelState;
    public TunnelState(Transform switchTransform)
    {
        context.Rigidbody.MovePosition(switchTransform.position);
    }

    public override void Enter()
    {
        base.Enter();
        OnSetTunnelState?.Invoke(true);
    }

    public override void Update()
    {
        context.Rigidbody.velocity = Vector2.right * context.Input.Axis.x * context.Values.XVelocity.NotSprintValue;
    }

    public override void Exit()
    {
        base.Exit();
        context.Rigidbody.transform.Translate(Vector2.up);
        OnSetTunnelState?.Invoke(false);
    }
}
