using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

//TODO: Rebuild Transition System to be more reliable while flexible
public class PlayerStateTransition : PlayerStateObject
{
    PlayerStateBase currentState;
    PlayerStateBase targetState;
    CheckType toCheckFor;
    ControlType inputNeeded;

    public PlayerStateTransition(PlayerContext playerContext, PlayerStateBase toTransitionTo, CheckType toCheckFor, ControlType inputNeeded, PlayerStateBase needsState = null) : base()
    {
        this.currentState = needsState;
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
        if (Active && notInTargetState && (StateIs(currentState)))
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
            {
                PlayerInputActionRegister.Instance.UnregisterInputAction(current);
                current = null;
            }
        }

        return false;
    }

    internal void TryExit()
    {

    }
}
