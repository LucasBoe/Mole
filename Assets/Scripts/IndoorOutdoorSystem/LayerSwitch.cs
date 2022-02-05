using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: make this not tunnel exclusive
public class LayerSwitch : PlayerAboveInputActionProvider
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

    protected override bool DidNotJustSpawn()
    {
        return true;
    }

    protected override void OnPlayerEnter()
    {
        UpdateInputActions();
        base.OnPlayerEnter();
    }

    private void TryInteract()
    {
        enableTime = Time.time;
        LayerHandler.Instance.SetLayer(layerLeadsTo);
    }

    private void TrySpy()
    {
        Layers sourceLayer = LayerHandler.Instance.GetCurrentLayer();
        PlayerStateMachine.Instance.SetState(new SpyingState(sourceLayer, layerLeadsTo));
    }
}
