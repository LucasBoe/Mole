using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatTriggerEnemyPull : PlayerCombatStateTrigger<EnemyPullState>
{
    protected override InputAction CreateInputAction()
    {
        return new InputAction() { Input = ControlType.Interact, Stage = InputActionStage.WorldObject, Text = "Pull", ActionCallback = EnterCombatState };
    }

    protected override PlayerStateBase CreateStateInstance(ICombatTarget combatTarget)
    {
        return new EnemyPullState(combatTarget, PlayerStateMachine.Instance.CurrentState);
    }

    protected override bool ConditionsMet(ICombatTarget combatTarget)
    {
        return true;
    }
}
