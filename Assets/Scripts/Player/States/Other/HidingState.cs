using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System.Linq;
using System;

public class HidingState : PlayerStaticState
{
    public virtual float HiddenValue => 0;
    public HidingState(IStaticTargetProvider targetProvider) : base()
    {
        target = targetProvider;
    }

    public override void Enter()
    {
        base.Enter();
        SetHiding(true);
    }

    public override void Exit()
    {
        base.Exit();
        SetHiding(false);
    }

    protected virtual void SetHiding(bool isHiding)
    {
        if (isHiding)
            SetHidingMode(PlayerHidingHandler.HidingMode.StateStatic);
        else
            SetHidingMode(PlayerHidingHandler.HidingMode.Auto);
    }
}
