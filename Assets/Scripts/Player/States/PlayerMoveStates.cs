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

    public override void Update()
    {
        base.Update();

        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown && !context.TriesMoveLeftRight)
            SetState(new WallState());
    }
}


public class IdleState : MoveBaseState
{
    public bool IsCrouching;
    public bool IsAtWall;
    float dropDownTimer = 0f;

    public override void Enter()
    {
        IsAtWall = IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight);
        dropDownTimer = 0;
    }

    public override void Update()
    {
        base.Update();

        IsCrouching = !context.Input.Sprinting;

        if (context.Input.Axis.x != 0 && !triesMovingIntoWall)
            SetState(new WalkState());

        //jumping
        if (context.Input.Jump)
            SetState(new JumpState());

        //dropping down
        if (IsColliding(CheckType.DropDownable) && context.Input.Axis.y < -0.9f)
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
            dropDownTimer = 0f;
        }
    }
}
public class WalkState : MoveBaseState
{
    public bool IsSprinting;

    public WalkState() : base() { }
    public override void Update()
    {
        base.Update();

        IsSprinting = context.Input.Sprinting;
        float xInput = context.Input.Axis.x;

        context.Rigidbody.velocity = new Vector2(xInput * context.Values.XVelocity.GetValue(context.Input), context.Rigidbody.velocity.y);

        if (xInput == 0 || triesMovingIntoWall)
            SetState(new IdleState());

        if (!IsColliding(CheckType.Ground))
            SetState(new FallState());

        if (context.Input.Jump)
            SetState(new JumpState());

        if (!StateIs(typeof(WalkPushState)) && ((xInput < 0 && IsColliding(CheckType.PushableLeft)) || (xInput > 0 && IsColliding(CheckType.PushableRight))))
            SetState(new WalkPushState());

    }
}

public class WalkPushState : WalkState
{
    public WalkPushState() : base() { }
    public override void Update()
    {
        base.Update();

        if (!IsColliding(CheckType.PushableLeft) && !IsColliding(CheckType.PushableRight))
            SetState(new WalkState());
    }
}

public class JumpState : MoveBaseState
{
    public JumpState() : base() { }

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
    public FallState() : base() { }

    public override void Enter()
    {
        startFallTime = Time.time;
        attackEnemyBelowAction = new InputAction()
        {
            Target = context.PlayerController.transform,
            Input = ControlType.Use,
            Stage = InputActionStage.WorldObject,
            Text = "Strangle",
            ActionCallback = () =>
            {
                ICombatTarget[] targets = GetCheck(CheckType.EnemyBelow).Get<ICombatTarget>();
                if (targets != null && targets.Length > 0)
                {
                    context.CombatTarget = targets[0];
                    SetState(new CombatStrangleState(targets[0]));
                }
            }
        };
    }

    public override void Update()
    {
        base.Update();

        //combat
        if (UpdateAttackEnemyBelow())
        {
            ICombatTarget[] targets = GetCheck(CheckType.EnemyBelow).Get<ICombatTarget>();
            if (targets != null && targets.Length > 0)
            {
                context.CombatTarget = targets[0];
                SetState(new CombatStrangleState(targets[0]));
            }
        }

        //gravity
        ApplyGravity(Time.time - startFallTime);


        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.Axis.y > 0.25f)
            SetState(new HangingState());

        if ((IsColliding(CheckType.HangableJumpInLeft) && context.Input.Axis.x < 0.1f) || (IsColliding(CheckType.HangableJumpInRight) && context.Input.Axis.x > -0.1f))
            SetState(new JumpToHangingState());

        //autograp to wall
        if (triesMovingIntoWall)
            SetState(new WalkState());

        bool isCollidingEdgeHelperLeft = IsColliding(CheckType.EdgeHelperLeft);
        bool isCollidingEdgeHelperRight = IsColliding(CheckType.EdgeHelperRight);
        bool isCollidingEdgeHelper = isCollidingEdgeHelperLeft || isCollidingEdgeHelperRight;
        bool isNotCollidingWall = !IsColliding(CheckType.WallLeft, CheckType.WallRight);
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

    //TODO: Move this into transition logic
    private bool UpdateAttackEnemyBelow()
    {
        //show / hide prompt
        bool enemyWasBelowBefore = enemyIsBelow;
        enemyIsBelow = IsColliding(CheckType.EnemyBelow);

        //handle time warp and attack indicator
        if (enemyIsBelow && !enemyWasBelowBefore)
        {
            Time.timeScale = 0.5f;
            PlayerInputActionRegister.Instance.RegisterInputAction(attackEnemyBelowAction);
        }
        else if (!enemyIsBelow && enemyWasBelowBefore && attackPrompt != null)
        {
            Time.timeScale = 1f;
            PlayerInputActionRegister.Instance.UnregisterInputAction(attackEnemyBelowAction);
        }

        return false;
    }

    public override void Exit()
    {
        base.Exit();
        PlayerInputActionRegister.Instance.UnregisterInputAction(attackEnemyBelowAction);
        Time.timeScale = 1f;
    }
}
