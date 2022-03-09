using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatState : PlayerStateBase
{
    protected ICombatTarget target;
    public PlayerCombatState(ICombatTarget target) : base()
    {
        this.target = target;
    }

    public override void Update()
    {
        if (target.IsNull)
            ExitCombat();
    }

    protected void ExitCombat()
    {
        SetState(new IdleState());
    }
}
