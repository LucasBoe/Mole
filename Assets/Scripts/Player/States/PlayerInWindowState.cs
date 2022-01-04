using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWindowState : PlayerStateBase
{
    public InWindowState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        if (context.Input.Axis.x != 0)
            SetState(PlayerState.Walk);
    }

}
