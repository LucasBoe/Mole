using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCombatTriggerStrangle : PlayerCombatStateTrigger<StrangleState>
{
    bool hiddenEnough = false;
    protected override InputAction CreateInputAction()
    {
        return new InputAction()
        {
            Target = transform,
            Input = ControlType.Use,
            Stage = InputActionStage.WorldObject,
            Text = "Strangle",
            ActionCallback = EnterCombatState
        };
    }
    private void OnEnable()
    {
        PlayerHidingHandler.ChangedHiddenValue += OnChangedHiddenValue;
    }
    private void OnDisable()
    {
        PlayerHidingHandler.ChangedHiddenValue -= OnChangedHiddenValue;
    }

    protected override void OnCombatTargetEnter(ICombatTarget target)
    {
        target.Memory.AlertedEnter += UpdateTrigger;
        target.Memory.AlertedExit += UpdateTrigger;
    }

    protected override void OnCombatTargetExit(ICombatTarget target)
    {
        target.Memory.AlertedEnter -= UpdateTrigger;
        target.Memory.AlertedExit -= UpdateTrigger;
    }


    private void OnChangedHiddenValue(float hidden)
    {
        Log("OnChangedHiddenValue");
        hiddenEnough = hidden < 0.4f;
        UpdateTrigger();
    }

    protected override PlayerStateBase CreateStateInstance(ICombatTarget combatTarget)
    {
        return new StrangleState(combatTarget);
    }

    protected override bool ConditionsMet(ICombatTarget combatTarget)
    {
        return hiddenEnough && combatTarget.IsAlive && !combatTarget.Memory.IsAlerted;
    }
}

