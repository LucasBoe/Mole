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

    protected void SetState(PlayerState state)
    {
        PlayerStateMachine.Instance.SetState(state);
    }

    protected bool StateIs(PlayerState state)
    {
        return PlayerStateMachine.Instance.CurrentState == state;
    }

    public PlayerStateObject(PlayerContext playerContext)
    {
        context = playerContext;
    }
}

//Player State Base Class with common and abstract functions 
public class PlayerStateBase : PlayerStateObject
{
    public PlayerStateBase(PlayerContext playerContext) : base(playerContext) { }

    protected PlayerStateTransitionChecks transitionCheck => context.StateTransitonChecks;

    //rope
    protected bool checkForRope = true;

    //hideable
    protected bool checkForHideable;


    public virtual void Enter() { }

    public virtual void Exit()
    {
        transitionCheck.Rope.TryExit();
    }

    public virtual void Update()
    {
        transitionCheck.Rope.TryCheck();
        transitionCheck.Hideable.TryCheck();
    }

    protected void SetCollisionActive(bool active)
    {
        context.PlayerController.SetCollisionActive(active);
    }
    protected void SetGravityActive(bool active)
    {
        context.Rigidbody.gravityScale = active ? 2 : 0;
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
}
