using PlayerCollisionCheckType;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Cimb States
public class PullUpState : PlayerState
{
    float t = 0;

    AnimationCurve curve;

    Vector2 startPos, targetPos;

    public PullUpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        curve = context.Values.PullUpCurve;
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

        if (t < context.Values.PullUpDuration)
        {
            context.Rigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, curve.Evaluate(t / context.Values.PullUpDuration)));
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
    private DropDownMode mode;

    IFloor[] floors;

    private enum DropDownMode
    {
        Hangable,
        Floor,
    }

    public DropDownState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        //Is the player dropping down from a hangable or through a oneDir. floor?
        floors = GetCheck(CheckType.DropDownable).GetFloors();
        mode = floors.Length > 0 ? DropDownMode.Floor : DropDownMode.Hangable;

        if (mode == DropDownMode.Hangable)
        {
            SetCollisionActive(false);
        }
        else
        {
            foreach (IFloor floor in floors)
                floor.DeactivateUntilPlayerIsAboveAgain(context.PlayerController);

            SetState(PlayerMoveState.Fall);
        }
    }
    public override void Update()
    {
        context.Rigidbody.MovePosition(context.PlayerPos + Vector2.down * Time.deltaTime * context.Values.DropDownSpeed);

        if (IsColliding(CheckType.Hangable))
            SetState(PlayerClimbState.Hanging);
    }
    public override void Exit()
    {
        SetCollisionActive(true);
    }
}
public class WallState : PlayerState
{
    public bool IsLeft => IsColliding(CheckType.WallLeft);

    public WallState(PlayerContext playerContext) : base(playerContext) { }


    public override void Update()
    {
        //up down movement
        context.Rigidbody.velocity = new Vector2(context.Rigidbody.velocity.x + (IsLeft ? -1f : 1f) * context.Values.WallPushVelocity, context.Input.y * context.Values.WallClimbYvelocity);

        //transition to hanging
        if (IsColliding(CheckType.Hangable)
            && ((!IsColliding(CheckType.WallLeft) && context.Input.x < 0) || (!IsColliding(CheckType.WallRight) && context.Input.x > 0)))
            SetState(PlayerClimbState.Hanging);

        //player loses connection to wall
        if (!context.IsCollidingToAnyWall)
            SetState(PlayerMoveState.Fall);
    }
}

public class HangingBaseState : PlayerState
{
    public HangingBaseState(PlayerContext playerContext) : base(playerContext) { }
    protected Vector2 GetClosestHangablePosition(Vector2 position, Vector2 input, CheckType[] checkTypes)
    {
        List<IHangable> hangables = new List<IHangable>();

        foreach (CheckType type in checkTypes)
        {
            foreach (IHangable hangable in GetCheck(type).GetHangables())
                hangables.Add(hangable);
        }

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

public class HangingState : HangingBaseState
{
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
            CheckType[] checkTypes = new CheckType[] { CheckType.Hangable, CheckType.HangableLeft, CheckType.HangableRight };
            Vector2 hangPosition = GetClosestHangablePosition(context.PlayerPos + context.Values.HangableOffset, context.Input * Time.deltaTime * 100f, checkTypes);
            Vector2 toMoveTo = hangPosition - context.Values.HangableOffset;

            Debug.DrawLine(context.PlayerPos, toMoveTo, Color.cyan);
            context.Rigidbody.MovePosition(Vector2.MoveTowards(context.PlayerPos, toMoveTo, Time.deltaTime * 10f));
        }
    }
}

public class JumpToHangingState : HangingBaseState
{
    Vector2 target;
    public JumpToHangingState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        SetCollisionActive(false);

        CheckType[] checkTypes = new CheckType[] { CheckType.HangableJumpInLeft, CheckType.HangableJumpInRight };
        target = GetClosestHangablePosition(context.PlayerPos, context.Input, checkTypes) - context.Values.HangableOffset;
    }

    public override void Update()
    {
        Vector2 pos = Vector2.MoveTowards(context.PlayerPos, target, Time.deltaTime * 30f);
        context.Rigidbody.MovePosition(pos);

        if (IsColliding(CheckType.Hangable))
            SetState(PlayerClimbState.Hanging);
    }

    public override void Exit()
    {
        SetCollisionActive(true);
    }
}
