using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

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

    protected void SetState(PlayerClimbState climbState)
    {
        context.PlayerController.SetState(climbState);
    }

    protected void SetState(PlayerMovementState moveState)
    {
        context.PlayerController.SetState(moveState);
    }

    protected void SetCollisionActive(bool active)
    {
        context.Rigidbody.GetComponent<Collider2D>().enabled = false;
    }

    public PlayerState(PlayerContext playerContext)
    {
        context = playerContext;
    }
}

public class PullUpState : PlayerState
{
    float pullUpSpeed = 8f;

    public PullUpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        SetCollisionActive(false);
    }
    public override void Update()
    {
        if (IsColliding(CheckType.Body))
            context.Rigidbody.MovePosition(context.PlayerPos + Vector2.up * Time.deltaTime * pullUpSpeed);
        else
            SetState(PlayerMovementState.Default);
    }
    public override void Exit()
    {
        SetCollisionActive(true);
    }
}

public class DropDownState : PlayerState
{
    float dropDownSpeed = 25f;

    public DropDownState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        SetCollisionActive(false);
    }
    public override void Update()
    {
        if (!IsColliding(CheckType.Hangable))
            context.Rigidbody.MovePosition(context.PlayerPos + Vector2.down * Time.deltaTime * dropDownSpeed);
        else
            SetState(PlayerClimbState.Hanging);
    }
    public override void Exit()
    {
        SetCollisionActive(true);
    }
}

public class WallState : PlayerState
{
    float wallClimbVelocity = 6;

    public WallState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        //up down movement
        context.Rigidbody.velocity = new Vector2(context.Rigidbody.velocity.x, context.Input.y * wallClimbVelocity);

        //transition to hanging
        if (IsColliding(CheckType.Hangable) && context.TriesMoveLeftRight)
            SetState(PlayerClimbState.Hanging);

        //player loses connection to wall
        if (!context.IsCollidingToAnyWall)
            SetState(PlayerMovementState.Default);
    }
}

public class HangingState : PlayerState
{
    public Vector2 HangableOffset = new Vector2(0, 1.25f);
    public HangingState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        //transition to wall climb
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown)
            SetState(PlayerClimbState.Wall);

        //pulling up
        if (!IsColliding(CheckType.HangableAboveAir) && context.Input.y > 0.5f && !context.TriesMoveLeftRight)
        {
            SetState(PlayerClimbState.PullUp);
        }
        else
        {
            Vector2 hangPosition = GetClosestHangablePosition(context.PlayerPos + HangableOffset, context.Input * Time.deltaTime * 100f);
            Vector2 toMoveTo = hangPosition - HangableOffset;

            Debug.DrawLine(context.PlayerPos, toMoveTo, Color.cyan);
            context.Rigidbody.MovePosition(Vector2.MoveTowards(context.PlayerPos, toMoveTo, Time.deltaTime * 10f));
        }
    }

    private Vector2 GetClosestHangablePosition(Vector2 position, Vector2 input)
    {
        List<IHangable> hangables = new List<IHangable>();

        foreach (IHangable hangable in context.CollisionChecks[CheckType.Hangable].GetHangables())
            hangables.Add(hangable);

        foreach (IHangable hangable in context.CollisionChecks[CheckType.HangableLeft].GetHangables())
            hangables.Add(hangable);

        foreach (IHangable hangable in context.CollisionChecks[CheckType.HangableRight].GetHangables())
            hangables.Add(hangable);

        float dist = float.MaxValue;
        Vector2 closest = Vector2.zero;

        foreach (IHangable hangable in hangables)
        {
            Vector2 c = hangable.GetClosestHangablePosition(position, input);
            float d = Vector2.Distance(position + input, c);

            if (d < dist)
            {
                dist = d;
                closest = c;
            }
        }

        if (closest != Vector2.zero)
        {
            Debug.DrawLine(closest + new Vector2(-0.1f, -0.1f), closest + new Vector2(0.1f, 0.1f), Color.green);
            Debug.DrawLine(closest + new Vector2(0.1f, -0.1f), closest + new Vector2(-0.1f, 0.1f), Color.green);
            return closest;
        }

        return position;

    }
}
