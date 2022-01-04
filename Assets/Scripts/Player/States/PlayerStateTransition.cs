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

    internal void TryCheck()
    {
        bool notInTargetState = !StateIs(targetState);
        if (Active && notInTargetState)
        {
            bool isColliding = IsColliding(toCheckFor) && notInTargetState;
            if (isColliding && !wasCollidingBefore)
            {
                if (prompt != null)
                    prompt.Hide();
                prompt = PlayerControlPromptUI.Show(inputNeeded, context.PlayerPos + Vector2.up);

            }
            else if (!isColliding && wasCollidingBefore && prompt != null)
                prompt.Hide();

            if (isColliding && context.Input.GetByControlType(inputNeeded) && notInTargetState)
                SetState(targetState);

            wasCollidingBefore = isColliding;
        }
        else
        {
            if (prompt != null)
                prompt.Hide();
        }
    }

    internal void TryExit()
    {

    }
}
