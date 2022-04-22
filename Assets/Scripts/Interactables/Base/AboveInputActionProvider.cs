using System;
using UnityEngine;

public class AboveInputActionProvider : AboveCooldownInteractable
{
    protected InputAction[] inputActions;

    protected virtual void Start()
    {
        UpdateInputActions();
    }

    protected void UpdateInputActions()
    {
        inputActions = CreateInputActions();
    }

    protected virtual InputAction[] CreateInputActions()
    {
        return null;
    }

    protected void RemoveAllInputActionsFor(UnityEngine.Object target)
    {
        PlayerInputActionRegister.Instance.UnregisterAllInputActions(target);
    }

    protected override void OnPlayerEnter()
    {
        PlayerInputActionRegister.Instance.RegisterInputActions(inputActions);            
    }

    protected override void OnPlayerExit()
    {
        PlayerInputActionRegister.Instance.UnregisterInputActions(inputActions);
    }

    private void OnDestroy()
    {
        OnPlayerExit();
    }
}
