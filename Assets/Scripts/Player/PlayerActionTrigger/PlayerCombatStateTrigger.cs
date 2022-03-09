using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerCombatStateTrigger<T> : PlayerActionTriggerBase where T : PlayerCombatState  
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

    protected override bool ConditionsMet(Collider2D collider2D)
    {
        ICombatTarget combatTarget = collider2D.GetComponent<ICombatTarget>();
        if (combatTarget != null)
            return ConditionsMet(combatTarget);

        return false;
    }

    protected abstract bool ConditionsMet(ICombatTarget combatTarget);
}
