using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System.Linq;
using System;

public class HidingState : PlayerStaticState
{
    public static System.Action EnterState, ExitState;
    public virtual float HiddenValue => 0;
    public HidingState(IStaticTargetProvider targetProvider) : base()
    {
        target = targetProvider;
    }

    public override void Enter()
    {
        base.Enter();
        EnterState?.Invoke();
        SetHiding(true);
    }

    public override void Exit()
    {
        base.Exit();
        ExitState?.Invoke();
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
