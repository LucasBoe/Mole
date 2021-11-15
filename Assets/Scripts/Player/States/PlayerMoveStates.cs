using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class MoveBaseState : PlayerStateBase
{
    string lastT = "";
    protected bool triesMovingIntoWall
    {
        get
        {
            bool wallToLeft = IsColliding(CheckType.WallLeft);
            bool wallToRight = IsColliding(CheckType.WallRight);
            return (wallToLeft && context.Input.x < 0) || (wallToRight && context.Input.x > 0);
        }
    }
    public MoveBaseState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown && !context.TriesMoveLeftRight)
            SetState(PlayerState.Wall);
    }
}


//Move States (Idle, Walk, Jump, Fall)
public class IdleState : MoveBaseState
{
    float dropDownTimer = 0f;

    public IdleState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        dropDownTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        if (context.Input.x != 0 && !triesMovingIntoWall)
            SetState(PlayerState.Walk);

        //jumping
        if (context.IsJumping)
            SetState(PlayerState.Jump);

        //dropping down
        if (IsColliding(CheckType.DropDownable) && context.Input.y < -0.9f)
        {
            dropDownTimer += Time.deltaTime;
            if (dropDownTimer > context.Values.KeyPressTimeToDropDown)
            {
                dropDownTimer = 0f;
                SetState(PlayerState.DropDown);
            }
        }
        else
        {
            dropDownTimer = 0f;
        }
    }
}
public class WalkState : MoveBaseState
{
    public WalkState(PlayerContext playerContext) : base(playerContext) { }
    public override void Update()
    {
        base.Update();

        float xInput = context.Input.x;

        context.Rigidbody.velocity = new Vector2(xInput * context.Values.walkXvelocity, context.Rigidbody.velocity.y);

        if (xInput == 0 || triesMovingIntoWall)
            SetState(PlayerState.Idle);

        if (!IsColliding(CheckType.Ground))
            SetState(PlayerState.Fall);

        if (context.IsJumping)
            SetState(PlayerState.Jump);

        if (!StateIs(PlayerState.WalkPush) && ((xInput < 0 && IsColliding(CheckType.PushableLeft)) || (xInput > 0 && IsColliding(CheckType.PushableRight))))
            SetState(PlayerState.WalkPush);
    }
}

public class WalkPushState : WalkState
{
    public WalkPushState(PlayerContext playerContext) : base(playerContext) { }
    public override void Update()
    {
        base.Update();

        if (!IsColliding(CheckType.PushableLeft) && !IsColliding(CheckType.PushableRight))
            SetState(PlayerState.Walk);
    }
}

public class JumpState : MoveBaseState
{
    public JumpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        Vector2 dir = context.Input.magnitude < 0.1f ? Vector2.up : context.Input;
        dir = Vector2.Lerp(Vector2.up, dir, context.Values.DirectionalJumpAmount);
        context.Rigidbody.AddForce(dir.normalized * context.Values.JumpForce, ForceMode2D.Impulse);
    }

    public override void Update()
    {
        base.Update();

        ApplyGravity(0f);

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.x * context.Values.StraveXVelocity, context.Rigidbody.velocity.y);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
            SetState(PlayerState.Hanging);

        if (context.Rigidbody.velocity.y < 0)
            SetState(PlayerState.Fall);
    }
}
public class FallState : MoveBaseState
{
    float startFallTime;
    public FallState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        startFallTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //gravity
        ApplyGravity(Time.time - startFallTime);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
            SetState(PlayerState.Hanging);

        if ((IsColliding(CheckType.HangableJumpInLeft) && context.Input.x < 0.1f) || (IsColliding(CheckType.HangableJumpInRight) && context.Input.x > -0.1f))
            SetState(PlayerState.JumpToHanging);

        if (triesMovingIntoWall)
            SetState(PlayerState.Wall);

        //strave
        if (context.TriesMoveLeftRight)
            context.Rigidbody.velocity = new Vector2(context.Input.x * context.Values.StraveXVelocity, context.Rigidbody.velocity.y);

        bool isCollidingEdgeHelper = IsColliding(CheckType.EdgeHelperLeft, CheckType.EdgeHelperRight);
        bool isNotCollidingWall = !IsColliding(CheckType.WallLeft, CheckType.WallRight);
        bool triesMovingUp = context.Input.y > 0.1f;

        if ((context.TriesMoveLeftRight || triesMovingUp) && isCollidingEdgeHelper && isNotCollidingWall)
        {
            Vector2 dir = Vector2.up;

            if (triesMovingUp)
            {
                dir += IsColliding(CheckType.EdgeHelperLeft) ? Vector2.left : Vector2.right;
                dir = dir.normalized;
            }

            context.Rigidbody.AddForce(dir * context.Values.EdgeHelperUpwardsImpulse, ForceMode2D.Impulse);
        }


        if (IsColliding(CheckType.Ground))
            SetState(PlayerState.Idle);
    }
}
