using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;

public class MoveBaseState : PlayerState
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
        if (context.Input.x != 0 && !triesMovingIntoWall)
            SetState(PlayerMoveState.Walk);

        //jumping
        if (context.IsJumping)
            SetState(PlayerMoveState.Jump);

        //dropping down
        if (IsColliding(CheckType.DropDownable) && context.Input.y < -0.9f)
        {
            dropDownTimer += Time.deltaTime;
            if (dropDownTimer > 0.5f)
            {
                dropDownTimer = 0f;
                SetState(PlayerClimbState.DropDown);
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
        float xInput = context.Input.x;

        context.Rigidbody.velocity = new Vector2(xInput * context.Values.walkXvelocity, context.Rigidbody.velocity.y);

        if (xInput == 0 || triesMovingIntoWall)
            SetState(PlayerMoveState.Idle);

        if (!IsColliding(CheckType.Ground))
            SetState(PlayerMoveState.Fall);

        if (context.IsJumping)
            SetState(PlayerMoveState.Jump);


    }
}
public class JumpState : MoveBaseState
{
    public JumpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        Vector2 dir = context.Input.magnitude < 0.1f ? Vector2.up : context.Input.normalized;
        context.Rigidbody.AddForce(dir * context.Values.JumpForce, ForceMode2D.Impulse);
    }

    public override void Update()
    {
        ApplyGravity(0f);

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.x * context.Values.StraveXVelocity, context.Rigidbody.velocity.y);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
            SetState(PlayerClimbState.Hanging);

        if (context.Rigidbody.velocity.y < 0)
            SetState(PlayerMoveState.Fall);
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
        //gravity
        ApplyGravity(Time.time - startFallTime);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
            SetState(PlayerClimbState.Hanging);

        if ((IsColliding(CheckType.HangableJumpInLeft) && context.Input.x < 0.1f) || (IsColliding(CheckType.HangableJumpInRight) && context.Input.x > -0.1f))
            SetState(PlayerClimbState.JumpToHanging);

        if (triesMovingIntoWall)
            SetState(PlayerClimbState.Wall);

        bool isCollidingEdgeHelper = IsColliding(CheckType.EdgeHelperLeft, CheckType.EdgeHelperRight);
        bool isNotCollidingWall = !IsColliding(CheckType.WallLeft, CheckType.WallRight);

        if ((context.TriesMoveLeftRight || context.Input.y > 0.1f) && isCollidingEdgeHelper && isNotCollidingWall)
        {
            context.Rigidbody.AddForce(Vector2.up * context.Values.EdgeHelperUpwardsImpulse, ForceMode2D.Impulse);
        }

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.x * context.Values.StraveXVelocity, context.Rigidbody.velocity.y);

        if (IsColliding(CheckType.Ground))
            SetState(PlayerMoveState.Idle);
    }
}
