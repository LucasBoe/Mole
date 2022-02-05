using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InWindowState : PlayerStateBase
{
    public InWindowState() : base() { }

    public override void Update()
    {
        if (context.Input.Axis.x != 0)
            SetState(new WalkState());
    }
}
