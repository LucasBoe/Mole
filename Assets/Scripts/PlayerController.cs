using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCollisionCheckType;
using UnityEngine;

public enum PlayerMovementState
{
    Idle,
    Walk,
    Run,
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
    [SerializeField] PlayerClimbState climbState;
    [SerializeField] PlayerMovementState moveState;
    [SerializeField] Vector2 hangableOffset;

    bool jumpBlocker = false;
    float dropDownTimer = 0f;

    Dictionary<CheckType, PlayerCollisionCheck> collisionChecks = new Dictionary<CheckType, PlayerCollisionCheck>();

    Rigidbody2D rigidbody;

    [SerializeField] private float jumpForce, walkForce, wallClimbForce, additionalGravityForce, pullUpSpeed, dropDownSpeed;

    public System.Action<PlayerMovementState, PlayerClimbState> OnStateChange;
    public System.Action<PlayerMovementState, PlayerClimbState, PlayerMovementState, PlayerClimbState> OnStateChangePrevious;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();

        //base
        collisionChecks.Add(CheckType.Ground, new PlayerCollisionCheck(0f, -1.125f, 0.75f, 0.25f, LayerMask.GetMask("Default", "Hangable")));
        collisionChecks.Add(CheckType.Hangable, new PlayerCollisionCheck(0f, 1.5f, 1.5f, 1f, LayerMask.GetMask("Hangable")));
        collisionChecks.Add(CheckType.WallLeft, new PlayerCollisionCheck(-0.625f, 0.5f, 0.25f, 1f, LayerMask.GetMask("Default")));
        collisionChecks.Add(CheckType.WallRight, new PlayerCollisionCheck(0.625f, 0.5f, 0.25f, 1f, LayerMask.GetMask("Default")));
        collisionChecks.Add(CheckType.Ceiling, new PlayerCollisionCheck(0f, 1f, 0.75f, 0.25f, LayerMask.GetMask("Default")));
        collisionChecks.Add(CheckType.Body, new PlayerCollisionCheck(0f, 0, 1f, 2f, LayerMask.GetMask("Default", "Hangable")));

        //details
        collisionChecks.Add(CheckType.HangableLeft, new PlayerCollisionCheck(-0.75f, 1.5125f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        collisionChecks.Add(CheckType.HangableRight, new PlayerCollisionCheck(0.75f, 1.5125f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        collisionChecks.Add(CheckType.HangableAboveAir, new PlayerCollisionCheck(0f, 3f, 1f, 2f, LayerMask.GetMask("Default", "Hangable")));
        collisionChecks.Add(CheckType.HangableBelow, new PlayerCollisionCheck(0, -1.25f, 0.5f, 1.5f, LayerMask.GetMask("Hangable")));
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerCollisionCheck pcc in collisionChecks.Values)
        {
            pcc.Update(transform);
        }

        Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        bool triesMoveLeftRight = input.x != 0;
        bool triesMoveUpDown = input.y != 0f;
        bool isJumping = Input.GetButton("Jump");
        bool isCollidingToAnyWall = IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight);

        switch (moveState)
        {
            case PlayerMovementState.Climb:

                //player tries to walk on the ground
                if (IsColliding(CheckType.Ground) && triesMoveLeftRight)
                    SetState(PlayerMovementState.Walk);

                if (isJumping)
                    JumpOff(input);

                switch (climbState)
                {
                    case PlayerClimbState.Wall:
                        //up down movement
                        rigidbody.velocity = new Vector2(rigidbody.velocity.x, input.y * wallClimbForce);

                        //transition to hanging
                        if (IsColliding(CheckType.Hangable) && triesMoveLeftRight)
                            SetState(PlayerClimbState.Hanging);

                        //player loses connection to wall
                        if (!isCollidingToAnyWall)
                            SetState(PlayerMovementState.Walk);

                        break;

                    case PlayerClimbState.Hanging:

                        //clamp xinput by hangable
                        float xinput = Mathf.Clamp(input.x, IsColliding(CheckType.HangableLeft) ? -1 : 0, IsColliding(CheckType.HangableRight) ? 1 : 0);
                        Vector2 hangPosition = GetClosestHangablePosition((Vector2)transform.position + hangableOffset, input * Time.deltaTime * 100f);
                        rigidbody.MovePosition(Vector2.MoveTowards(transform.position, hangPosition - hangableOffset, Time.deltaTime * 10f));

                        //transition to wall climb
                        if (isCollidingToAnyWall && triesMoveUpDown)
                            SetState(PlayerClimbState.Wall);

                        //pulling up
                        if (!IsColliding(CheckType.HangableAboveAir) && input.y > 0.9f && !triesMoveLeftRight)
                        {
                            SetState(PlayerClimbState.PullUp);
                        }

                        break;

                    case PlayerClimbState.PullUp:
                        if (IsColliding(CheckType.Body))
                            rigidbody.MovePosition((Vector2)transform.position + Vector2.up * Time.deltaTime * pullUpSpeed);
                        else
                            SetState(PlayerMovementState.Walk);
                        break;

                    case PlayerClimbState.DropDown:
                        if (!IsColliding(CheckType.Hangable))
                            rigidbody.MovePosition((Vector2)transform.position + Vector2.down * Time.deltaTime * dropDownSpeed);
                        else
                            SetState(PlayerClimbState.Hanging);
                        break;
                }

                break;

            default:

                if (input.x != 0)
                    rigidbody.velocity = new Vector2(input.x * walkForce, rigidbody.velocity.y);

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
                        rigidbody.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                        Invoke("ResetJumpBlocker", 0.5f);
                    }

                    //dropping down
                    if (IsColliding(CheckType.HangableBelow) && input.y < -0.9f)
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
                    rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * additionalGravityForce));

                    //autograp to hangable
                    if (IsColliding(CheckType.Hangable) && input.y > 0.25f)
                        SetState(PlayerClimbState.Hanging);
                }
                break;
        }
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
                        rigidbody.GetComponent<Collider2D>().enabled = true;
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
        rigidbody.gravityScale = moveState == PlayerMovementState.Climb ? 0 : 2;

        switch (moveState)
        {
            case PlayerMovementState.Walk:
                dropDownTimer = 0f;
                break;

            case PlayerMovementState.Climb:

                switch (climbState)
                {
                    case PlayerClimbState.PullUp:
                    case PlayerClimbState.DropDown:
                        rigidbody.GetComponent<Collider2D>().enabled = false;
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
        SetState(PlayerMovementState.Walk);
        rigidbody.velocity = input;
    }

    private Vector2 GetClosestHangablePosition(Vector2 position, Vector2 input)
    {
        Debug.DrawLine(position, position + input, Color.red);

        List<IHangable> hangables = new List<IHangable>();

        foreach (IHangable hangable in collisionChecks[CheckType.Hangable].GetHangables())
            hangables.Add(hangable);

        foreach (IHangable hangable in collisionChecks[CheckType.HangableLeft].GetHangables())
            hangables.Add(hangable);

        foreach (IHangable hangable in collisionChecks[CheckType.HangableRight].GetHangables())
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
            Debug.DrawLine(position + input, closest, Color.cyan);
            Debug.DrawLine(closest + new Vector2(-0.1f, -0.1f), closest + new Vector2(0.1f, 0.1f), Color.green);
            Debug.DrawLine(closest + new Vector2(0.1f, -0.1f), closest + new Vector2(-0.1f, 0.1f), Color.green);
            return closest;
        }

        return position;

    }

    public void SetState(PlayerClimbState climbState)
    {
        SetState(PlayerMovementState.Climb, climbState);
    }

    public void SetState(PlayerMovementState newMoveState, PlayerClimbState newClimbState = PlayerClimbState.None)
    {

        ExitState(moveState, climbState);
        EnterState(newMoveState, newClimbState);

        OnStateChangePrevious?.Invoke(moveState, climbState, newMoveState, newClimbState);
        OnStateChange?.Invoke(newMoveState, newClimbState);

        moveState = newMoveState;
        climbState = newClimbState;

    }

    private bool IsColliding(CheckType checkType)
    {
        return collisionChecks[checkType].IsDetecting;
    }

    private void OnDrawGizmos()
    {
        foreach (PlayerCollisionCheck pcc in collisionChecks.Values)
        {
            Gizmos.color = pcc.IsDetecting ? Color.yellow : Color.white;
            Gizmos.DrawWireCube((Vector2)transform.position + pcc.Pos, pcc.Size);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + hangableOffset, 0.25f);
    }

    private void OnGUI()
    {
        foreach (KeyValuePair<CheckType, PlayerCollisionCheck> pcc in collisionChecks)
        {
            Vector3 offsetFromSize = new Vector3(0.1f + ((Vector3)(pcc.Value.Size / 2f)).x, ((Vector3)(pcc.Value.Size / 2f)).y);
            Vector2 screenPos = Camera.main.WorldToScreenPoint(transform.position + (Vector3)pcc.Value.Pos + offsetFromSize);
            Rect rect = new Rect(screenPos.x, Screen.height - screenPos.y, 150, 50);
            GUI.Label(rect, pcc.Key.ToString() + " (" + pcc.Value.LayerMask.ToString() + ")");
        }
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
