using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatStateTrigger<T> : PlayerActionTriggerBase where T : PlayerCombatState  
{
    public virtual void EnterCombatState()
    {
        if (ActionTarget == null)
            return;

        ICombatTarget combatTarget = ActionTarget.GetComponent<ICombatTarget>();

        if (combatTarget != null)
            PlayerStateMachine.Instance.SetState(CreateStateInstance(combatTarget));
    }

    protected virtual PlayerStateBase CreateStateInstance(ICombatTarget combatTarget)
    {
        return null;
    }
}
