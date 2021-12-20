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
            bool wallToLeft = IsColliding(CheckType.WallLeft);
            bool wallToRight = IsColliding(CheckType.WallRight);
            return (wallToLeft && context.Input.Axis.x < 0) || (wallToRight && context.Input.Axis.x > 0);
        }
    }
    public MoveBaseState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown && !context.TriesMoveLeftRight)
            SetState(PlayerState.Wall);
    }
}


public class IdleState : MoveBaseState
{
    public bool IsAtWall;
    float dropDownTimer = 0f;

    public IdleState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        IsAtWall = IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight);
        dropDownTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        if (context.Input.Axis.x != 0 && !triesMovingIntoWall)
            SetState(PlayerState.Walk);

        //jumping
        if (context.Input.Jump)
            SetState(PlayerState.Jump);

        //dropping down
        if (IsColliding(CheckType.DropDownable) && context.Input.Axis.y < -0.9f)
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
    public bool IsSprinting;

    public WalkState(PlayerContext playerContext) : base(playerContext) { }
    public override void Update()
    {
        base.Update();

        IsSprinting = context.Input.HoldingSprint;
        float xInput = context.Input.Axis.x;

        context.Rigidbody.velocity = new Vector2(xInput * (IsSprinting ? context.Values.walkXvelocity : context.Values.crouchXvelocity), context.Rigidbody.velocity.y);

        if (xInput == 0 || triesMovingIntoWall)
            SetState(PlayerState.Idle);

        if (!IsColliding(CheckType.Ground))
            SetState(PlayerState.Fall);

        if (context.Input.Jump)
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
        Vector2 dir = context.Input.Axis.magnitude < 0.1f ? Vector2.up : context.Input.Axis;
        dir = Vector2.Lerp(Vector2.up, dir, context.Values.DirectionalJumpAmount);
        context.Rigidbody.AddForce(dir.normalized * context.Values.JumpForce, ForceMode2D.Impulse);
    }

    public override void Update()
    {
        base.Update();

        ApplyGravity(0f);

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.Axis.x * context.Values.StraveXVelocity, context.Rigidbody.velocity.y);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.Axis.y > 0.25f)
            SetState(PlayerState.Hanging);

        if (context.Rigidbody.velocity.y < 0)
            SetState(PlayerState.Fall);
    }
}
public class FallState : MoveBaseState
{
    PlayerControlPromptUI attackPrompt;
    bool enemyIsBelow;
    float startFallTime;
    public FallState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        startFallTime = Time.time;
    }

    public override void Update()
    {
        base.Update();

        //combat
        if (UpdateAttackEnemyBelow())
            SetState(PlayerState.CombatStrangle);

        //gravity
        ApplyGravity(Time.time - startFallTime);


        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.Axis.y > 0.25f)
            SetState(PlayerState.Hanging);

        if ((IsColliding(CheckType.HangableJumpInLeft) && context.Input.Axis.x < 0.1f) || (IsColliding(CheckType.HangableJumpInRight) && context.Input.Axis.x > -0.1f))
            SetState(PlayerState.JumpToHanging);

        //autograp to wall
        if (triesMovingIntoWall)
            SetState(PlayerState.Wall);

        bool isCollidingEdgeHelperLeft = IsColliding(CheckType.EdgeHelperLeft);
        bool isCollidingEdgeHelperRight = IsColliding(CheckType.EdgeHelperRight);
        bool isCollidingEdgeHelper = isCollidingEdgeHelperLeft || isCollidingEdgeHelperRight;
        bool isNotCollidingWall = !IsColliding(CheckType.WallLeft, CheckType.WallRight);
        bool triesMovingUp = context.Input.Axis.y > 0.1f;

        //strave
        if (context.TriesMoveLeftRight)
        {
            float clampedInputX = Mathf.Clamp(context.Input.Axis.x, isCollidingEdgeHelperLeft ? 0f : -1f, isCollidingEdgeHelperRight ? 0f : 1f);
            context.Rigidbody.velocity = new Vector2(clampedInputX * context.Values.StraveXVelocity, context.Rigidbody.velocity.y);
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
            SetState(PlayerState.Idle);
    }

    private bool UpdateAttackEnemyBelow()
    {
        //show / hide prompt
        bool enemyWasBelowBefore = enemyIsBelow;
        enemyIsBelow = IsColliding(CheckType.EnemyBelow);

        //handle time warp and attack indicator
        if (enemyIsBelow && !enemyWasBelowBefore)
        {
            Time.timeScale = 0.5f;
            attackPrompt = PlayerControlPromptUI.Show(ControlType.Use, context.PlayerPos + Vector2.down);
        }
        else if (!enemyIsBelow && enemyWasBelowBefore && attackPrompt != null)
        {
            Time.timeScale = 1f;
            attackPrompt.Hide();
        }

        //handle attack input
        if (enemyIsBelow && context.Input.Use)
        {
            ICombatTarget[] targets = GetCheck(CheckType.EnemyBelow).Get<ICombatTarget>();
            if (targets != null && targets.Length > 0)
            {
                context.CombatTarget = targets[0];
                return true;
            }
        }

        return false;
    }

    public override void Exit()
    {
        base.Exit();
        if (attackPrompt != null)
            attackPrompt.Hide();

        Time.timeScale = 1f;
    }
}
