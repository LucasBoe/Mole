using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAboveInputActionProvider : PlayerAboveInteractable
{
    private InputAction inputAction;

    protected override void OnEnable()
    {
        base.OnEnable();
        inputAction = CreateInputAction();
    }

    protected virtual InputAction CreateInputAction()
    {
        return null;
    }

    protected override void OnPlayerEnter()
    {
        if (inputAction == null)
            Debug.LogWarning("No input action found on " + name);
        else
            PlayerInputActionRegister.Instance.RegisterInputAction(inputAction);
    }

    protected override void OnPlayerExit()
    {
        if (inputAction == null)
            Debug.LogWarning("No input action found on " + name);
        else
            PlayerInputActionRegister.Instance.UnregisterInputAction(inputAction);
    }
}
