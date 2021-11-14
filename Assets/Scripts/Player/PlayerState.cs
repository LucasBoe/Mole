using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

//Player State Base Class with common and abstract functions 
public class PlayerState
{
    protected PlayerContext context;

    public virtual void Enter() { }

    public virtual void Exit() { }

    public virtual void Update() { }

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

    protected void SetState(PlayerClimbState climbState)
    {
        context.PlayerController.SetState(climbState);
    }
    protected void SetState(PlayerMoveState moveState)
    {
        context.PlayerController.SetState(moveState);
    }

    protected void SetState(PlayerBaseState baseState)
    {
        context.PlayerController.SetState(baseState);
    }

    protected bool StateIs(PlayerMoveState moveState)
    {
        return context.PlayerController.MoveState == moveState;
    }

    protected void SetCollisionActive(bool active)
    {
        context.Rigidbody.GetComponent<Collider2D>().enabled = active;
    }

    protected void JumpOff(Vector2 input)
    {
        context.Rigidbody.velocity = input * context.Values.JumpOffVelocity;
        SetState(PlayerMoveState.Fall);
    }

    protected void ApplyGravity(float time)
    {
        context.Rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * context.Values.AdditionalGravityForce.Evaluate(Mathf.Clamp(time,0f,4f))));
    }

    public PlayerState(PlayerContext playerContext)
    {
        context = playerContext;
    }
}

//Base States
public class DefaultState : PlayerState
{
    public DefaultState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        context.PlayerController.EnterState(context.PlayerController.MoveState);
    }

    public override void Update()
    {
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown && !context.TriesMoveLeftRight)
            SetState(PlayerClimbState.Wall);

        context.PlayerController.UpdateState(context.PlayerController.MoveState);
    }

    public override void Exit()
    {
        context.PlayerController.ExitState(context.PlayerController.MoveState);
    }
}
public class ClimbState : PlayerState
{
    public ClimbState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        context.Rigidbody.gravityScale = 0;
        context.PlayerController.EnterState(context.PlayerController.ClimbState);
    }

    public override void Update()
    {
        //player tries to walk on the ground transition to Idle
        if (IsColliding(CheckType.Ground) && context.TriesMoveLeftRight && !context.TriesMoveUpDown)
            SetState(PlayerMoveState.Idle);

        //jump
        if (context.IsJumping)
            JumpOff(context.Input);

        context.PlayerController.UpdateState(context.PlayerController.ClimbState);
    }

    public override void Exit()
    {
        context.PlayerController.ExitState(context.PlayerController.ClimbState);
        context.Rigidbody.gravityScale = 2;
    }
}
