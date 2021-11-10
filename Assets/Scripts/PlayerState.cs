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

    protected void SetCollisionActive(bool active)
    {
        context.Rigidbody.GetComponent<Collider2D>().enabled = active;
    }

    protected void JumpOff(Vector2 input)
    {
        SetState(PlayerMoveState.Fall);
        context.Rigidbody.velocity = input;
    }

    protected void ApplyGravity()
    {
        context.Rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * 3));
    }

    public PlayerState(PlayerContext playerContext)
    {
        context = playerContext;
    }
}

//Base States
public class DefaultState : PlayerState
{
    float walkForce = 8f, jumpForce = 30f, additionalGravityForce = 3f;

    float lastJumpTime = 0;

    bool jumpBlocker => Time.time - 0.2f < lastJumpTime;

    public DefaultState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        context.PlayerController.EnterState(context.PlayerController.MoveState);
    }

    public override void Update()
    {
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown)
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
        //player tries to walk on the ground transition to Default
        if (IsColliding(CheckType.Ground) && context.TriesMoveLeftRight)
            SetState(PlayerBaseState.Default);

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


//Walk States (Idle, Walk, Jump, Fall)
public class IdleState : PlayerState
{
    float dropDownTimer = 0f;

    public IdleState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        dropDownTimer = 0;
    }

    public override void Update()
    {
        if (context.Input.x != 0)
            SetState(PlayerMoveState.Walk);

        //jumping
        if (context.IsJumping)
            SetState(PlayerMoveState.Jump);

        //dropping down
        if (IsColliding(CheckType.HangableBelow) && context.Input.y < -0.9f)
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
public class WalkState : PlayerState
{
    float walkForce = 8f;
    public WalkState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        context.Rigidbody.velocity = new Vector2(context.Input.x * walkForce, context.Rigidbody.velocity.y);

        if (context.Input.x == 0)
            SetState(PlayerMoveState.Idle);

        if (!IsColliding(CheckType.Ground))
            SetState(PlayerMoveState.Fall);

        if (context.IsJumping)
            SetState(PlayerMoveState.Jump);


    }
}
public class JumpState : PlayerState
{
    float jumpForce = 30f;
    float straveForce = 6f;
    public JumpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        context.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public override void Update()
    {
        //gravity
        ApplyGravity();

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.x * straveForce, context.Rigidbody.velocity.y);

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
            SetState(PlayerClimbState.Hanging);

        if (context.Rigidbody.velocity.y < 0)
            SetState(PlayerMoveState.Fall);
    }
}
public class FallState : PlayerState
{
    float straveForce = 6f;
    public FallState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        //gravity
        ApplyGravity();

        //autograp to hangable
        if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
            SetState(PlayerClimbState.Hanging);

        //strave
        context.Rigidbody.velocity = new Vector2(context.Input.x * straveForce, context.Rigidbody.velocity.y);

        if (IsColliding(CheckType.Ground))
            SetState(PlayerMoveState.Idle);
    }
}


//Cimb States
public class PullUpState : PlayerState
{
    float t = 0;
    float duration = 0.5f;

    AnimationCurve positionOverTimeCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    Vector2 startPos, targetPos;

    public PullUpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        SetCollisionActive(false);

        t = 0;
        int i = 0;

        Vector2 toCheck = context.PlayerPos + Vector2.up;
        bool blocked = true;
        while (blocked && i < 25)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(toCheck, new Vector2(1, 1.5f), 0, LayerMask.GetMask("Hangable"));
            blocked = colliders.Length != 0;
            Debug.DrawLine(toCheck + Vector2.left * 0.1f, toCheck + Vector2.right * 0.1f, blocked ? Color.red : Color.green, 10);
            toCheck += new Vector2(0, 0.25f);
            i++;
        }

        startPos = context.PlayerPos;
        targetPos = toCheck;
    }
    public override void Update()
    {
        t += Time.deltaTime;

        if (t < duration)
        {
            context.Rigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, positionOverTimeCurve.Evaluate(t / duration)));
        }
        else
            SetState(PlayerMoveState.Idle);
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
            SetState(PlayerMoveState.Fall);
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
