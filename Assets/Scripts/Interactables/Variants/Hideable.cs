using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//TODO: Generalize this to not need a separate layer but share it with other action providers
public class Hideable : Interactable, IMoveableStaticTargetProvider
{
    [SerializeField] Transform inputActionTransform;
    [SerializeField] private bool allowMovement = false;
    [SerializeField] private Rigidbody2D movementBody;

    public bool IsActive => isActiveAndEnabled;
    private bool isHiding = false;

    public InputAction GetCustomExitAction() { return null; }
    public Transform GetTransform() { return inputActionTransform; }

    public void Move(Vector2 velocity)
    {
        if (movementBody != null)
        {
            movementBody.velocity = velocity;
        }
    }

    public bool ProvidesCustomActionCallback() { return false; }

    protected override InputAction[] CreateInputActions()
    {
        return new InputAction[] { new InputAction()
        {
            Input = ControlType.Use,
            Target = inputActionTransform,
            Stage= InputActionStage.WorldObject,
            Text = "Hide",
            ActionCallback = Hide
        }};
    }

    protected override void OnPlayerEnter()
    {
        if (!isHiding)
            base.OnPlayerEnter();
    }

    private void Hide()
    {
        HidingState.ExitState += OnStopHiding;
        OnPlayerExit();
        isHiding = true;
        PlayerStateMachine.Instance.SetState(allowMovement ? new HidingCanMoveState(this) : new HidingState(this));
    }

    private void OnStopHiding()
    {
        HidingState.ExitState -= OnStopHiding;
        OnPlayerEnter();
        isHiding = false;
    }
}
