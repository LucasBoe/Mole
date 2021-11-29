using PlayerCollisionCheckType;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbStateBase : PlayerStateBase
{
    public ClimbStateBase(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        context.Rigidbody.gravityScale = 0;
    }

    public override void Update()
    {
        //player tries to walk on the ground transition to Idle
        if (IsColliding(CheckType.Ground) && context.TriesMoveLeftRight && !context.TriesMoveUpDown)
            SetState(PlayerState.Walk);

        //jump
        if (context.Input.Jump)
            JumpOff(context.Input.Axis);
    }

    public override void Exit()
    {
        context.Rigidbody.gravityScale = 2;
    }
}


//Cimb States
public class PullUpState : ClimbStateBase
{
    float t = 0;

    AnimationCurve curve;

    Vector2 startPos, targetPos;

    public PullUpState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        base.Enter();

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
            SetState(PlayerState.Idle);
    }
    public override void Exit()
    {
        base.Exit();
        SetCollisionActive(true);
    }
}
public class DropDownState : ClimbStateBase
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
        base.Enter();

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

            SetState(PlayerState.Fall);
        }
    }
    public override void Update()
    {
        context.Rigidbody.MovePosition(context.PlayerPos + Vector2.down * Time.deltaTime * context.Values.DropDownSpeed);

        if (IsColliding(CheckType.Hangable))
            SetState(PlayerState.Hanging);
    }
    public override void Exit()
    {
        base.Exit();
        SetCollisionActive(true);
    }
}
public class WallState : ClimbStateBase
{
    public bool IsMoving;
    public float DistanceFromTop;
    public bool IsLeft => IsColliding(CheckType.WallLeft);

    public WallState(PlayerContext playerContext) : base(playerContext) { }

    public override void Update()
    {
        base.Update();

        //Calculate Parameters for animation
        DistanceFromTop = 0;
        if (!IsColliding(CheckType.WallAbove))
        {
            Vector2 origin = context.PlayerPos + Vector2.up + (IsLeft ? Vector2.left : Vector2.right);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 2, LayerMask.GetMask("Default"));
            if (hit == true)
            {
                DistanceFromTop = Vector2.Distance(origin, hit.point);
                Debug.DrawLine(origin, hit.point, Color.red);
            }
        }
        IsMoving = context.Rigidbody.velocity.y != 0;

        //transition to wall stretch
        bool triesMoveAwayFromWall = ((IsLeft && context.Input.Axis.x > 0) || (!IsLeft && context.Input.Axis.x < 0));
        if (triesMoveAwayFromWall)
        {
            RaycastHit2D hit = Physics2D.Raycast(context.PlayerPos, new Vector2(IsLeft ? -1 : 1, 0), 2, LayerMask.GetMask("Default"));
            if (hit == true)
            {
                SetState(PlayerState.WallStretch);
            }
        }

        //up down movement
        context.Rigidbody.velocity = new Vector2(context.Rigidbody.velocity.x + (IsLeft ? -1f : 1f) * context.Values.WallPushVelocity, context.Input.Axis.y * context.Values.WallClimbYvelocity);

        //transition to hanging
        if (IsColliding(CheckType.Hangable)
            && ((!IsColliding(CheckType.WallLeft) && context.Input.Axis.x < 0) || (!IsColliding(CheckType.WallRight) && context.Input.Axis.x > 0)))
            SetState(PlayerState.Hanging);

        //player loses connection to wall
        if (!context.IsCollidingToAnyWall)
            SetState(PlayerState.Fall);
    }
}

public class WallStretchState : ClimbStateBase
{
    float enterStateX = 0;

    public float Distance = 0;

    public WallStretchState(PlayerContext playerContext) : base(playerContext) { }

    public override void Enter()
    {
        base.Enter();
        enterStateX = context.PlayerPos.x;
    }

    public override void Update()
    {
        base.Update();

        float directionToWall = enterStateX < context.PlayerPos.x ? -1f : 1f;

        RaycastHit2D hit = Physics2D.Raycast(context.PlayerPos, new Vector2(directionToWall, 0), 2, LayerMask.GetMask("Default"));
        if (hit == true)
        {
            Distance = Vector2.Distance(context.PlayerPos, hit.point);
            Debug.DrawLine(context.PlayerPos, hit.point, Color.red);
        }

        float minimalWallDistance = 0.55f;
        bool moveUpDownThenLeftRight = Mathf.Abs(context.Input.Axis.y) > Mathf.Abs(context.Input.Axis.x);

        //tries to move up down
        if (moveUpDownThenLeftRight)
        {
            context.Rigidbody.velocity = new Vector2(directionToWall * context.Values.WallSnapXVelocity * (Distance - minimalWallDistance), context.Input.Axis.y * context.Values.WallClimbYvelocity * 0.5f);
        }
        //clamped left right
        else
        {
            bool reachedMax = Distance > 1.375f;
            float floatClampedX = Mathf.Clamp(context.Input.Axis.x, (reachedMax && directionToWall > 0) ? 0 : -1, (reachedMax && directionToWall < 0) ? 0 : 1);
            context.Rigidbody.velocity = new Vector2(floatClampedX * context.Values.WallClimbYvelocity, context.Rigidbody.velocity.y);
        }

        if (moveUpDownThenLeftRight && context.IsCollidingToAnyWall && Distance < 0.65)
            SetState(PlayerState.Wall);
    }
}

public class HangingBaseState : ClimbStateBase
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
        base.Update();

        //transition to wall climb
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown)
            SetState(PlayerState.Wall);

        //pulling up
        if (!IsColliding(CheckType.HangableAboveAir) && context.Input.Axis.y > 0.5f && !context.TriesMoveLeftRight)
        {
            SetState(PlayerState.PullUp);
        }
        else
        {
            CheckType[] checkTypes = new CheckType[] { CheckType.Hangable, CheckType.HangableLeft, CheckType.HangableRight };
            Vector2 hangPosition = GetClosestHangablePosition(context.PlayerPos + context.Values.HangableOffset, context.Input.Axis * Time.deltaTime * 100f, checkTypes);
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
        base.Enter();

        SetCollisionActive(false);

        CheckType[] checkTypes = new CheckType[] { CheckType.HangableJumpInLeft, CheckType.HangableJumpInRight };
        target = GetClosestHangablePosition(context.PlayerPos, context.Input.Axis, checkTypes) - context.Values.HangableOffset;
    }

    public override void Update()
    {
        base.Update();

        Vector2 pos = Vector2.MoveTowards(context.PlayerPos, target, Time.deltaTime * 30f);
        context.Rigidbody.MovePosition(pos);

        if (IsColliding(CheckType.Hangable))
            SetState(PlayerState.Hanging);
    }

    public override void Exit()
    {
        base.Exit();
        SetCollisionActive(true);
    }
}
