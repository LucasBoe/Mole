using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

public class TunnelState : PlayerStateBase
{
    TunnelUser tunnelUser;
    public TunnelState() : base()
    {
        tunnelUser = TunnelUser.Instance;
    }

    public override void Enter()
    {
        base.Enter();

        //LayerSwitch entrance = GetCheck(CheckType.Tunnel).Get<LayerSwitch>()[0];
        //context.Rigidbody.MovePosition(entrance.transform.position);
    }

    public override void Update()
    {
        context.Rigidbody.velocity = Vector2.right * context.Input.Axis.x * context.Values.XVelocity.NotSprintValue;
    }
}
