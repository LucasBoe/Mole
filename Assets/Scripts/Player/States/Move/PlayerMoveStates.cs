using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System;

public class MoveBaseState : PlayerStateBase
{
    protected bool triesMovingIntoWall
    {
        get
        {
            bool wallToLeft = IsColliding(CheckType.ClimbableLeft);
            bool wallToRight = IsColliding(CheckType.ClimbableRight);
            return (wallToLeft && context.Input.Axis.x < 0) || (wallToRight && context.Input.Axis.x > 0);
        }
    }

    public override void Update()
    {
        base.Update();

        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown && !context.TriesMoveLeftRight)
            SetState(new GutterClimbState());
    }
}


public class IdleState : MoveBaseState
{
    public bool IsCrouching;
    public bool IsAtWall;
    float dropDownTimer = 0f;

    public override void Enter()
    {
        IsAtWall = IsColliding(CheckType.ClimbableLeft) || IsColliding(CheckType.ClimbableRight);
        dropDownTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        if (RopeClimbState.TryEnter())
            return;

        IsCrouching = !context.Input.Sprinting;

        //jumping
        if (context.Input.Jump)
            SetState(new JumpState());


        CollisionCheck dropDown = GetCheck(CheckType.DropDownable);
        float dropAngle = Util.GetAngleFromHangable(dropDown, context);

        //dropping down
        if (dropDown.IsDetecting && dropAngle <= 45f && context.Input.Axis.y < -0.1f)
        {
            dropDownTimer += Time.deltaTime;
            if (dropDownTimer > context.Values.KeyPressTimeToDropDown)
            {
                dropDownTimer = 0f;
                IFloor[] floors = GetCheck(CheckType.DropDownable).GetFloors();
                SetState(new DropDownState(floors));
            }
        }
        else
        {
            dropAngle = 0f;
            dropDownTimer = 0f;
        }

        if (dropAngle == 0f && context.Input.Axis.x != 0 && !triesMovingIntoWall)
            SetState(new WalkState());
    }
}
public class WalkState : MoveBaseState
{
    public bool IsSprinting;

    public static System.Action<bool> Walk;

    public override void Enter()
    {
        Walk?.Invoke(true);
    }

    public override void Update()
    {
        base.Update();

        IsSprinting = context.Input.Sprinting && !IsHoldingHeavyItem();
        float x = context.Input.Axis.x * context.Values.XVelocity.GetValue(IsSprinting);
        context.Rigidbody.velocity = new Vector2(x, context.Rigidbody.velocity.y);

        if (!context.TriesMoveLeftRight || triesMovingIntoWall)
            SetState(new IdleState());

        if (!IsColliding(CheckType.Ground))
            SetState(new FallState());

        if (context.Input.Jump)
            SetState(new JumpState());
    }

    public override void Exit()
    {
        Walk?.Invoke(false);
    }
}

public class JumpState : MoveBaseState
{
    public JumpState() : base() { }
    public override bool StateAllowsCarryingHeavyObjects => false;

    public override void Enter()
    {
        Vector2 dir = context.Input.Axis.magnitude < 0.1f ? Vector2.up : context.Input.Axis;
        dir = Vector2.Lerp(Vector2.up, dir, context.Input.Sprinting ? context.Values.DirectionalJumpAmount : 0);
        context.Rigidbody.AddForce(dir.normalized * context.Values.JumpForce.GetValue(context.Input), ForceMode2D.Impulse);
    }

    public override void Update()
    {
        base.Update();

        ApplyGravity(0f);

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.Axis.x * context.Values.StraveXVelocity.GetValue(context.Input), context.Rigidbody.velocity.y);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.Axis.y > 0.25f)
            SetState(new HangingState());

        if (context.Rigidbody.velocity.y < 0)
            SetState(new FallState());
    }
}
public class FallState : MoveBaseState
{
    PlayerControlPromptUI attackPrompt;
    bool enemyIsBelow;
    float startFallTime;
    InputAction attackEnemyBelowAction;

    public static System.Action<float> ExitFallState;

    public override void Enter()
    {
        startFallTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        if (RopeClimbState.TryEnter())
            return;

        //gravity
        ApplyGravity(Time.time - startFallTime);

        //fall death
        if ((Time.time - startFallTime) > 5f)
            PlayerHealth.Instance.DoDamage(1);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.Axis.y > 0.25f)
            SetState(new HangingState());

        if ((IsColliding(CheckType.HangableJumpInLeft) && context.Input.Axis.x < 0.1f) || (IsColliding(CheckType.HangableJumpInRight) && context.Input.Axis.x > -0.1f))
            SetState(new JumpToHangingState());

        //autograp to wall
        if (triesMovingIntoWall)
            SetState(new GutterClimbState());

        bool isCollidingEdgeHelperLeft = IsColliding(CheckType.EdgeHelperLeft);
        bool isCollidingEdgeHelperRight = IsColliding(CheckType.EdgeHelperRight);
        bool isCollidingEdgeHelper = isCollidingEdgeHelperLeft || isCollidingEdgeHelperRight;
        bool isNotCollidingWall = !IsColliding(CheckType.AdditionalWallCheck);
        bool triesMovingUp = context.Input.Axis.y > 0.1f;

        //strave
        if (context.TriesMoveLeftRight)
        {
            float clampedInputX = Mathf.Clamp(context.Input.Axis.x, isCollidingEdgeHelperLeft ? 0f : -1f, isCollidingEdgeHelperRight ? 0f : 1f);
            context.Rigidbody.velocity = new Vector2(clampedInputX * context.Values.StraveXVelocity.GetValue(context.Input), context.Rigidbody.velocity.y);
        }


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
            SetState(new IdleState());
    }

    public override void Exit()
    {
        ExitFallState?.Invoke(startFallTime);
        base.Exit();
    }
}
