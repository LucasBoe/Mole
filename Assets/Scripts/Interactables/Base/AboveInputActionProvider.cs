using System;

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

    protected override void OnPlayerEnter()
    {
        foreach (var inputAction in inputActions)
        {
            PlayerInputActionRegister.Instance.RegisterInputAction(inputAction);
        }
            
    }

    protected override void OnPlayerExit()
    {
        foreach (var inputAction in inputActions)
        {
            PlayerInputActionRegister.Instance.UnregisterInputAction(inputAction);
        }
    }

    private void OnDestroy()
    {
        OnPlayerExit();
    }
}
