using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

public class PlayerStateObject
{
    protected PlayerContext context;
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

    protected void SetState(PlayerStateBase state)
    {
        PlayerStateMachine.Instance.SetState(state);
    }

    protected bool StateIs(PlayerStateBase state)
    {
        return StateIs(state.GetType());
    }

    public bool StateIs(System.Type stateType)
    {
        return PlayerStateMachine.Instance.CurrentState.GetType() == stateType;
    }

    public PlayerStateObject()
    {
        context = PlayerController.Context;
    }
}

//Player State Base Class with common and abstract functions 
public class PlayerStateBase : PlayerStateObject
{

    protected PlayerStateTransitionChecks transitionCheck => context.StateTransitonChecks;

    //rope
    protected bool checkForRope = true;

    //hideable
    protected bool checkForHideable;


    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }
    protected void SetCollisionActive(bool active)
    {
        PlayerPhysicsModifier.Instance.SetCollisionActive(active);
    }
    protected void SetGravityActive(bool active)
    {
        PlayerPhysicsModifier.Instance.SetGracityActive(active);
    }

    protected void SetPlayerDragActive(bool active)
    {
        PlayerPhysicsModifier.Instance.SetPlayerDragActive(active);
    }

    protected void JumpOff(Vector2 input)
    {
        context.Rigidbody.velocity = input * context.Values.JumpOffVelocity;
        SetState(new FallState());
    }
    protected void ApplyGravity(float time)
    {
        context.Rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * context.Values.AdditionalGravityForce.Evaluate(Mathf.Clamp(time, 0f, 4f))));
    }
}
