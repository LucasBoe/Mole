using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

//Player State Base Class with common and abstract functions 
public class PlayerStateBase
{
    PlayerControlPromptUI useRopeControlPrompt;
    bool wasAboveClimbableRopeBefore;

    protected PlayerContext context;

    public virtual void Enter() { }

    public virtual void Exit()
    {
        if (useRopeControlPrompt != null)
            useRopeControlPrompt.Hide();
    }

    public virtual void Update()
    {
        bool stateIsNotRopeClimb = !StateIs(PlayerState.RopeClimb);
        bool isAboveClimbableRope = IsColliding(CheckType.Rope) && stateIsNotRopeClimb;
        if (isAboveClimbableRope && !wasAboveClimbableRopeBefore)
        {
            if (useRopeControlPrompt != null)
                useRopeControlPrompt.Hide();
            useRopeControlPrompt = PlayerControlPromptUI.Show(ControlType.Use, context.PlayerPos + Vector2.up);

        }
        else if (!isAboveClimbableRope && wasAboveClimbableRopeBefore && useRopeControlPrompt != null)
            useRopeControlPrompt.Hide();

        if (isAboveClimbableRope && context.Input.Use && stateIsNotRopeClimb)
            SetState(PlayerState.RopeClimb);

        wasAboveClimbableRopeBefore = isAboveClimbableRope;
    }

    protected bool IsColliding(CheckType type)
    {
        return context.CollisionChecks[type].IsDetecting;
    }

    protected bool IsColliding(params CheckType[] types)
    {
        foreach (var type in types)
        {
            if (context.CollisionChecks[type].IsDetecting)
                return true;
        }

        return false;
    }
    protected CollisionCheck GetCheck(CheckType type)
    {
        return context.CollisionChecks[type];
    }

    protected void SetState(PlayerState state)
    {
        PlayerStateMachine.Instance.SetState(state);
    }

    protected bool StateIs(PlayerState state)
    {
        return PlayerStateMachine.Instance.CurrentState == state;
    }

    protected void SetCollisionActive(bool active)
    {
        context.PlayerController.SetCollisionActive(active);
    }

    protected void JumpOff(Vector2 input)
    {
        context.Rigidbody.velocity = input * context.Values.JumpOffVelocity;
        SetState(PlayerState.Fall);
    }

    protected void ApplyGravity(float time)
    {
        context.Rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * context.Values.AdditionalGravityForce.Evaluate(Mathf.Clamp(time, 0f, 4f))));
    }

    public PlayerStateBase(PlayerContext playerContext)
    {
        context = playerContext;
    }
}
