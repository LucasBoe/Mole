using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TunnelState : PlayerStateBase
{
    TunnelUser tunnelUser;
    public TunnelState(PlayerContext playerContext) : base(playerContext)
    {
        tunnelUser = TunnelUser.Instance;
    }

    public override void Update()
    {
        context.Rigidbody.velocity = Vector2.right * context.Input.Axis.x * context.Values.TunnelXvelocity;
    }
}
