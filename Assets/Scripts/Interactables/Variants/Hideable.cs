using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Generalize this to not need a separate layer but share it with other action providers
public class Hideable : AboveInputActionProvider, IStaticTargetProvider
{
    public InputAction GetCustomExitAction() { return null; }
    public Transform GetTransform() { return transform; }
    public bool ProvidesCustomActionCallback() { return false; }

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction()
        {
            Input = ControlType.Use,
            Target = transform,
            Stage= InputActionStage.WorldObject,
            Text = "Hide",
            ActionCallback = Hide
        }};
    }

    private void Hide()
    {
        PlayerStateMachine.Instance.SetState(new HidingState(this));
    }
}
