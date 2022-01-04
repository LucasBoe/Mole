using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

public class PlayerStateTransition : PlayerStateObject
{
    PlayerState targetState;
    CheckType toCheckFor;
    ControlType inputNeeded;


    public PlayerStateTransition(PlayerContext playerContext, PlayerState toTransitionTo, CheckType toCheckFor, ControlType inputNeeded) : base(playerContext)
    {
        this.targetState = toTransitionTo;
        this.toCheckFor = toCheckFor;
        this.inputNeeded = inputNeeded;
    }
    public bool Active = true;

    PlayerControlPromptUI prompt;
    bool wasCollidingBefore;

    private InputAction current;

    internal bool TryCheck()
    {
        bool notInTargetState = !StateIs(targetState);
        if (Active && notInTargetState)
        {
            bool isColliding = IsColliding(toCheckFor) && notInTargetState;
            if (isColliding && !wasCollidingBefore)
            {
                IInputActionProvider inputActionProvider = GetCheck(toCheckFor).Get<IInputActionProvider>()[0];
                current = inputActionProvider.FetchInputAction();
                current.Input = inputNeeded;
                PlayerInputActionRegister.Instance.RegisterInputAction(current);
            }
            else if (!isColliding && wasCollidingBefore && current != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);

            wasCollidingBefore = isColliding;
        }
        else
        {
            if (current != null)
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);
        }

        return false;
    }

    internal void TryExit()
    {

    }
}
