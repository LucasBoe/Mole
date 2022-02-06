using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: make this not tunnel exclusive
public class LayerSwitch : AboveInputActionProvider
{
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Layers layerLeadsTo;
    PlayerControlPromptUI prompt;
    InputAction enterAction, spyAction;

    protected override void OnEnable()
    {
        enterAction = new InputAction()
        {
            Text = "Enter",
            Target = transform,
            ActionCallback = TryInteract,
            Stage = InputActionStage.WorldObject
        };

        spyAction = new InputAction()
        {
            Input = ControlType.Use,
            Text = "Spy",
            Target = transform,
            ActionCallback = TrySpy,
            Stage = InputActionStage.WorldObject
        };

        base.OnEnable();
    }

    protected override InputAction[] CreateInputActions()
    {
        bool isSpying = PlayerStateMachine.Instance.CurrentState.GetType() == typeof(SpyingState);

        if (isSpying)
        {
            return new InputAction[] { enterAction };
        }
        else
        {
            return new InputAction[] { enterAction, spyAction };
        }
    }
    protected override void OnPlayerEnter()
    {
        UpdateInputActions();
        base.OnPlayerEnter();

    }
    private void TryInteract()
    {
        LayerHandler.Instance.SwitchLayer(layerLeadsTo);
        UpdatePlayerState(layerLeadsTo == Layers.Tunnels);
        PlayerInputActionRegister.Instance.UnregisterInputAction(spyAction);
        PlayerInputActionRegister.Instance.UnregisterInputAction(enterAction);

    }
    private void TrySpy()
    {
        PlayerStateBase currentState = PlayerStateMachine.Instance.CurrentState;
        PlayerStateMachine.Instance.SetState(new SpyingState(currentState, LayerHandler.Instance.CurrentLayer, layerLeadsTo, enterAction));

        PlayerInputActionRegister.Instance.UnregisterInputAction(spyAction);
    }

    private void UpdatePlayerState(bool enterTunnel)
    {
        if (enterTunnel)
            PlayerStateMachine.Instance.SetState(new TunnelState(transform));
        else
        {
            PlayerStateMachine.Instance.transform.Translate(Vector2.up);
            PlayerStateMachine.Instance.SetState(new IdleState());
        }
    }

}
