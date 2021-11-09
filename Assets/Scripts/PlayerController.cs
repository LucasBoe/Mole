using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCollisionCheckType;
using UnityEngine;

public enum PlayerMovementState
{
    Default,
    Climb,
}

public enum PlayerClimbState
{
    None,
    Wall,
    Ceiling,
    Hanging,
    PullUp,
    DropDown,
}

public class PlayerController : MonoBehaviour
{
    public PlayerClimbState ClimbState;
    public PlayerMovementState MoveState;

    PlayerContext context;

    bool jumpBlocker = false;
    float dropDownTimer = 0f;

    [SerializeField] private float jumpForce, walkForce, wallClimbForce, additionalGravityForce, pullUpSpeed, dropDownSpeed;

    public System.Action<PlayerMovementState, PlayerClimbState> OnStateChange;
    public System.Action<PlayerMovementState, PlayerClimbState, PlayerMovementState, PlayerClimbState> OnStateChangePrevious;

    public Dictionary<PlayerClimbState, PlayerState> climbStateDictionary = new Dictionary<PlayerClimbState, PlayerState>();

    private void Awake()
    {
        context = new PlayerContext();
        context.Rigidbody = GetComponent<Rigidbody2D>();
        context.PlayerController = this;

        //base
        context.CollisionChecks.Add(CheckType.Ground, new PlayerCollisionCheck(0f, -1.125f, 0.75f, 0.25f, LayerMask.GetMask("Default", "Hangable")));
        context.CollisionChecks.Add(CheckType.Hangable, new PlayerCollisionCheck(0f, 1.5f, 1.5f, 1f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.WallLeft, new PlayerCollisionCheck(-0.65f, 0.4f, 0.25f, 1f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.WallRight, new PlayerCollisionCheck(0.625f, 0.4f, 0.25f, 1f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.Ceiling, new PlayerCollisionCheck(0f, 1f, 0.75f, 0.25f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.Body, new PlayerCollisionCheck(0f, 0f, 1f, 1.5f, LayerMask.GetMask("Default", "Hangable")));

        //details
        context.CollisionChecks.Add(CheckType.HangableLeft, new PlayerCollisionCheck(-0.75f, 1.5125f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableRight, new PlayerCollisionCheck(0.75f, 1.5125f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableAboveAir, new PlayerCollisionCheck(0f, 3f, 1f, 2f, LayerMask.GetMask("Default", "Hangable")));
        context.CollisionChecks.Add(CheckType.HangableBelow, new PlayerCollisionCheck(0, -1.25f, 0.5f, 1f, LayerMask.GetMask("Hangable")));

        climbStateDictionary.Add(PlayerClimbState.PullUp, new PullUpState(context));
        climbStateDictionary.Add(PlayerClimbState.DropDown, new DropDownState(context));
        climbStateDictionary.Add(PlayerClimbState.Hanging, new HangingState(context));
        climbStateDictionary.Add(PlayerClimbState.Wall, new WallState(context));
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerCollisionCheck pcc in context.CollisionChecks.Values)
        {
            pcc.Update(transform);
        }

        context.PlayerPos = transform.position;
        context.Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        context.IsCollidingToAnyWall = IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight);
        context.TriesMoveLeftRight = context.Input.x != 0;
        context.TriesMoveUpDown = context.Input.y != 0f;
        context.IsJumping = Input.GetButton("Jump");

        bool triesMoveLeftRight = context.TriesMoveLeftRight;
        bool triesMoveUpDown = context.TriesMoveUpDown;
        bool isJumping = context.IsJumping;
        bool isCollidingToAnyWall = context.IsCollidingToAnyWall;

        switch (MoveState)
        {
            case PlayerMovementState.Climb:

                //player tries to walk on the ground
                if (IsColliding(CheckType.Ground) && triesMoveLeftRight)
                    SetState(PlayerMovementState.Default);

                if (isJumping)
                    JumpOff(context.Input);

                switch (ClimbState)
                {
                    case PlayerClimbState.Wall:
                    case PlayerClimbState.Hanging:
                    case PlayerClimbState.PullUp:
                    case PlayerClimbState.DropDown:
                        UpdateState(ClimbState);
                        break;
                }

                break;

            default:

                if (context.Input.x != 0)
                    context.Rigidbody.velocity = new Vector2(context.Input.x * walkForce, context.Rigidbody.velocity.y);

                if (isCollidingToAnyWall && triesMoveUpDown)
                {
                    SetState(PlayerClimbState.Wall);
                }

                if (IsColliding(CheckType.Ground))
                {
                    //jumping
                    if (!jumpBlocker && isJumping)
                    {
                        jumpBlocker = true;
                        context.Rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        Invoke("ResetJumpBlocker", 0.5f);
                    }

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
                else
                {
                    //gravity
                    context.Rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * additionalGravityForce));

                    //autograp to hangable
                    if (IsColliding(CheckType.Hangable) && context.Input.y > 0.25f)
                        SetState(PlayerClimbState.Hanging);
                }
                break;
        }
    }

    private void UpdateState(PlayerClimbState climbState)
    {
        climbStateDictionary[climbState].Update();
    }

    private void ExitState(PlayerMovementState moveState, PlayerClimbState climbState)
    {
        switch (moveState)
        {
            case PlayerMovementState.Climb:

                switch (climbState)
                {
                    case PlayerClimbState.PullUp:
                    case PlayerClimbState.DropDown:
                        context.Rigidbody.GetComponent<Collider2D>().enabled = true;
                        break;

                    default:
                        //
                        break;
                }
                break;
        }
    }

    private void EnterState(PlayerMovementState moveState, PlayerClimbState climbState)
    {
        context.Rigidbody.gravityScale = moveState == PlayerMovementState.Climb ? 0 : 2;
        dropDownTimer = 0f;

        switch (moveState)
        {
            case PlayerMovementState.Default:
                break;

            case PlayerMovementState.Climb:

                switch (climbState)
                {
                    case PlayerClimbState.PullUp:
                    case PlayerClimbState.DropDown:
                        climbStateDictionary[climbState].Enter();
                        break;

                    default:
                        //
                        break;
                }
                break;
        }
    }

    private void JumpOff(Vector2 input)
    {
        SetState(PlayerMovementState.Default);
        context.Rigidbody.velocity = input;
    }

    public void SetState(PlayerClimbState climbState)
    {
        SetState(PlayerMovementState.Climb, climbState);
    }

    public void SetState(PlayerMovementState newMoveState, PlayerClimbState newClimbState = PlayerClimbState.None)
    {

        ExitState(MoveState, ClimbState);
        EnterState(newMoveState, newClimbState);

        Debug.Log($"Change state from: {MoveState} ({ClimbState}) to {newMoveState}({newClimbState}).");
        OnStateChangePrevious?.Invoke(MoveState, ClimbState, newMoveState, newClimbState);
        OnStateChange?.Invoke(newMoveState, newClimbState);

        MoveState = newMoveState;
        ClimbState = newClimbState;

    }

    private bool IsColliding(CheckType checkType)
    {
        return context.CollisionChecks[checkType].IsDetecting;
    }

    private void OnDrawGizmos()
    {
        foreach (PlayerCollisionCheck pcc in context.CollisionChecks.Values)
        {
            Gizmos.color = pcc.IsDetecting ? Color.yellow : Color.white;
            Gizmos.DrawWireCube((Vector2)transform.position + pcc.Pos, pcc.Size);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + (climbStateDictionary[PlayerClimbState.Hanging] as HangingState).HangableOffset, 0.25f);
    }

    private void OnGUI()
    {
        //foreach (KeyValuePair<CheckType, PlayerCollisionCheck> pcc in context.CollisionChecks)
        //{
        //    Vector3 offsetFromSize = new Vector3(0.1f + ((Vector3)(pcc.Value.Size / 2f)).x, ((Vector3)(pcc.Value.Size / 2f)).y);
        //    Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)pcc.Value.Pos + offsetFromSize);
        //    Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, 150, 50);
        //    GUI.Label(rect, pcc.Key.ToString() + " (" + pcc.Value.LayerMask.ToString() + ")");
        //}

        GUILayout.Box(MoveState + " : " + ClimbState);
    }

    private void ResetJumpBlocker()
    {
        jumpBlocker = false;
    }
}

namespace PlayerCollisionCheckType
{
    public enum CheckType
    {
        Ground,
        WallLeft,
        WallRight,
        Ceiling,
        Hangable,
        HangableLeft,
        HangableRight,
        HangableAboveAir,
        Body,
        HangableBelow,
    }
}
