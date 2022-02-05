using System;

public class PlayerAboveInputActionProvider : PlayerAboveInteractable
{
    private InputAction[] inputActions;

    protected override void OnEnable()
    {
        base.OnEnable();
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
