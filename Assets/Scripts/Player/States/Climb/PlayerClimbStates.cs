using PlayerCollisionCheckType;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ClimbStateBase : PlayerStateBase
{
    public override bool CheckEnter()
    {
        return (!PlayerItemUser.Instance.BlocksClimb());
    }

    public ClimbStateBase() : base() { }

    public override void Enter()
    {
        SetGravityActive(false);
    }

    public override void Update()
    {
        //player tries to walk on the ground transition to Idle
        if (IsColliding(CheckType.Ground) && context.TriesMoveLeftRight && !context.TriesMoveUpDown)
            SetState(new WalkState());

        //jump
        if (context.Input.Jump)
            JumpOff(context.Input.Axis);
    }

    public override void Exit()
    {
        SetGravityActive(true);
    }
}

public class LadderClimbState : PlayerStateBase
{
    Ladder climbingLadder;
    Vector2 top, bottom;

    public new static bool CheckEnter()
    {
        PlayerStateMachine stateMachine = PlayerStateMachine.Instance;
        if (stateMachine.CurrentState.StateIs(typeof(LadderClimbState)))
            return false;

        if (Mathf.Abs(PlayerInputHandler.PlayerInput.Axis.y) < Mathf.Abs(PlayerInputHandler.PlayerInput.Axis.x))
            return false;

        return true;
    }

    public LadderClimbState(Ladder ladder)
    {
        climbingLadder = ladder;
        top = ladder.GetExitPointTop();
        bottom = ladder.GetExitPointBottom();
    }

    public override void Update()
    {
        base.Update();

        Vector2 pos = Util.GetClosestPointOnLineSegment(top, bottom, context.PlayerPos + context.Input.Axis);
        context.Rigidbody.MovePosition(Vector2.MoveTowards(context.PlayerPos, pos, context.Values.LadderClimbVelocity * Time.deltaTime));

        bool closeToTopOrBottom = Vector2.Distance(context.PlayerPos, top) < 0.2f || Vector2.Distance(context.PlayerPos, bottom) < 0.2f;

        if (closeToTopOrBottom && context.TriesMoveLeftRight)
            SetState(new WalkState());

        if (context.Input.Jump)
            JumpOff(context.Input.Axis);
    }

    public override void Enter()
    {
        SetCollisionActive(false);
        base.Enter();
    }

    public override void Exit()
    {
        SetCollisionActive(true);
        base.Exit();
    }
}


//Cimb States
public class PullUpState : ClimbStateBase
{
    float t = 0;
    int mask;

    AnimationCurve curve;

    Vector2 startPos, targetPos;

    public PullUpState() : base() { }

    public static void TryEnter(PlayerStateBase previous, PlayerContext context)
    {
        if (previous.IsColliding(CheckType.Hangable) && context.Input.Axis.y > 0.5f && !context.TriesMoveLeftRight)
            PlayerStateMachine.Instance.SetState(new PullUpState());
    }

    public override void Enter()
    {
        base.Enter();

        curve = context.Values.PullUpCurve;
        mask = LayerMask.GetMask("Hangable", "OneDirectionalFloor", "Climbable");
        SetCollisionActive(false);

        t = 0;
        int i = 0;

        Vector2 toCheck = context.PlayerPos + Vector2.up;
        bool blocked = true;
        while (blocked && i < 25)
        {
            Collider2D[] colliders = Physics2D.OverlapBoxAll(toCheck, new Vector2(1, 1.5f), 0, mask);
            blocked = colliders.Length != 0;
            Debug.DrawLine(toCheck + Vector2.left * 0.1f, toCheck + Vector2.right * 0.1f, blocked ? Color.red : Color.green, 10);
            toCheck += new Vector2(0, 0.25f);
            i++;
        }

        startPos = context.PlayerPos;
        targetPos = toCheck + Vector2.up * 0.5f; //+0.5 to counteract clothlines moving up;
    }
    public override void Update()
    {
        t += Time.deltaTime;

        var checkEmptyBoxPos = context.PlayerPos + new Vector2(0, 0.5f);
        var checkEmptyBoxScale = new Vector2(1, 2.5f);

        bool pullUpFinished = t >= context.Values.PullUpDuration;
        bool reachedStablePosition = !Physics2D.OverlapBox(checkEmptyBoxPos, checkEmptyBoxScale, 0, mask);

        if (!pullUpFinished && !reachedStablePosition)
        {
            Util.DebugDrawBox(checkEmptyBoxPos, checkEmptyBoxScale, color: Color.red);
            context.Rigidbody.MovePosition(Vector2.Lerp(startPos, targetPos, curve.Evaluate(t / context.Values.PullUpDuration)));
        }
        else
        {
            if (reachedStablePosition)
                Util.DebugDrawBox(checkEmptyBoxPos, checkEmptyBoxScale, color: Color.yellow, 3f);
            if (pullUpFinished)
                Util.DebugDrawBox(checkEmptyBoxPos + new Vector2(0.25f, 0.25f), checkEmptyBoxScale, color: Color.green, 3f);

            context.Rigidbody.velocity = Vector2.zero;
            SetState(new IdleState());
        }
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

    public DropDownState(IFloor[] floors) : base()
    {
        this.floors = floors;
    }

    public override void Enter()
    {
        base.Enter();

        //Is the player dropping down from a hangable or through a oneDir. floor?
        mode = floors.Length > 0 ? DropDownMode.Floor : DropDownMode.Hangable;

        if (mode == DropDownMode.Hangable)
        {
            SetCollisionActive(false);
        }
        else
        {
            foreach (IFloor floor in floors)
                floor.DeactivateUntilPlayerIsAboveAgain(context.PlayerController);

            SetState(new FallState());
        }
    }
    public override void Update()
    {
        context.Rigidbody.MovePosition(context.PlayerPos + Vector2.down * Time.deltaTime * context.Values.DropDownSpeed);

        if (IsColliding(CheckType.Hangable))
            SetState(new HangingState());
    }
    public override void Exit()
    {
        base.Exit();
        SetCollisionActive(true);
    }
}
public class GutterClimbState : ClimbStateBase
{
    public bool IsMoving;
    public float DistanceFromTop;
    public bool IsLeft => IsColliding(CheckType.WallLeft);

    public GutterClimbState() : base() { }

    public override void Update()
    {
        base.Update();

        //Calculate Parameters for animation
        DistanceFromTop = 0;
        if (!IsColliding(CheckType.WallAbove))
        {
            Vector2 origin = context.PlayerPos + Vector2.up + (IsLeft ? Vector2.left : Vector2.right);
            RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, 2, LayerMask.GetMask("Climbable"));
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
            RaycastHit2D hit = Physics2D.Raycast(context.PlayerPos, new Vector2(IsLeft ? -1 : 1, 0), 2, LayerMask.GetMask("Climbable"));
            if (hit == true)
            {
                SetState(new GutterStretchState());
            }
        }

        //up down movement
        context.Rigidbody.velocity = new Vector2(context.Rigidbody.velocity.x + (IsLeft ? -1f : 1f) * context.Values.WallPushVelocity, context.Input.Axis.y * context.Values.WallClimbYvelocity);

        //transition to hanging
        HangingState.TryEnter(this, context);

        //transition to pullup
        PullUpState.TryEnter(this, context);

        //player loses connection to wall
        if (!context.IsCollidingToAnyWall)
            SetState(new FallState());
    }
}

public class GutterStretchState : ClimbStateBase
{
    float enterStateX = 0;

    public float Distance = 0;

    public GutterStretchState() : base() { }

    public override void Enter()
    {
        base.Enter();
        enterStateX = context.PlayerPos.x;
    }

    public override void Update()
    {
        base.Update();

        float directionToWall = enterStateX < context.PlayerPos.x ? -1f : 1f;

        RaycastHit2D hit = Physics2D.Raycast(context.PlayerPos, new Vector2(directionToWall, 0), 2, LayerMask.GetMask("Climbable"));
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
            SetState(new GutterClimbState());

        //transition to hanging
        HangingState.TryEnter(this, context);
    }
}

public class HangingBaseState : ClimbStateBase
{
    public HangingBaseState() : base() { }
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
    PlayerPhysicsModifier.ColliderMode modeBefore;

    public static void TryEnter(PlayerStateBase before, PlayerContext context)
    {
        if (before.IsColliding(CheckType.Hangable)&& ((!before.IsColliding(CheckType.WallLeft) && context.Input.Axis.x < 0) || (!before.IsColliding(CheckType.WallRight) && context.Input.Axis.x > 0)))
            SetState(new HangingState());
    }

    public override void Enter()
    {
        base.Enter();
        modeBefore = PlayerPhysicsModifier.Instance.Mode;
        PlayerPhysicsModifier.Instance.SetColliderMode(PlayerPhysicsModifier.ColliderMode.Hanging);
    }

    public override void Update()
    {
        base.Update();

        //transition to wall climb
        if (context.IsCollidingToAnyWall && context.TriesMoveUpDown)
            SetState(new GutterClimbState());


        CheckType[] checkTypes = new CheckType[] { CheckType.Hangable, CheckType.HangableLeft, CheckType.HangableRight };
        Vector2 hangPosition = GetClosestHangablePosition(context.PlayerPos + context.Values.HangableOffset, context.Input.Axis * Time.deltaTime * 100f, checkTypes);
        Vector2 toMoveTo = hangPosition - context.Values.HangableOffset;

        Debug.DrawLine(context.PlayerPos, toMoveTo, Color.cyan);
        context.Rigidbody.MovePosition(Vector2.MoveTowards(context.PlayerPos, toMoveTo, Time.deltaTime * 10f));

        PullUpState.TryEnter(this, context);
    }

    public override void Exit()
    {
        base.Exit();
        PlayerPhysicsModifier.Instance.SetColliderMode(modeBefore);
    }
}

public class JumpToHangingState : HangingBaseState
{
    Vector2 target;
    public JumpToHangingState() : base() { }

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
            SetState(new HangingState());
    }

    public override void Exit()
    {
        base.Exit();
        SetCollisionActive(true);
    }
}
