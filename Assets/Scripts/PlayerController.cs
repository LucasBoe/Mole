using System;
using System.Collections;
using System.Collections.Generic;
using PlayerCollisionCheckType;
using UnityEngine;

public enum PlayerBaseState
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
    public PlayerBaseState BaseState;

    PlayerContext context;

    bool jumpBlocker = false;

    [SerializeField] private float jumpForce, walkForce, wallClimbForce, additionalGravityForce, pullUpSpeed, dropDownSpeed;

    public System.Action<PlayerBaseState, PlayerClimbState> OnStateChange;
    public System.Action<PlayerBaseState, PlayerClimbState, PlayerBaseState, PlayerClimbState> OnStateChangePrevious;

    public Dictionary<PlayerClimbState, PlayerState> climbStateDictionary = new Dictionary<PlayerClimbState, PlayerState>();
    public Dictionary<PlayerBaseState, PlayerState> baseStateDictionary = new Dictionary<PlayerBaseState, PlayerState>();

    private void Awake()
    {
        context = new PlayerContext();
        context.Rigidbody = GetComponent<Rigidbody2D>();
        context.PlayerController = this;

        //base
        context.CollisionChecks.Add(CheckType.Ground, new PlayerCollisionCheck(0f, -1.25f, 0.75f, 0.25f, LayerMask.GetMask("Default", "Hangable")));
        context.CollisionChecks.Add(CheckType.Hangable, new PlayerCollisionCheck(0f, 1.375f, 1.5f, 1f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.WallLeft, new PlayerCollisionCheck(-0.65f, 0.275f, 0.25f, 1f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.WallRight, new PlayerCollisionCheck(0.625f, 0.275f, 0.25f, 1f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.Ceiling, new PlayerCollisionCheck(0f, 0.875f, 0.75f, 0.25f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.Body, new PlayerCollisionCheck(0f, 0f, 0.875f, 1.5f, LayerMask.GetMask("Default", "Hangable")));

        //details
        context.CollisionChecks.Add(CheckType.HangableLeft, new PlayerCollisionCheck(-0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableRight, new PlayerCollisionCheck(0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableAboveAir, new PlayerCollisionCheck(0f, 2.875f, 1f, 2f, LayerMask.GetMask("Default", "Hangable")));
        context.CollisionChecks.Add(CheckType.HangableBelow, new PlayerCollisionCheck(0, -1.5f, 0.5f, 1f, LayerMask.GetMask("Hangable")));

        //base states
        baseStateDictionary.Add(PlayerBaseState.Default, new DefaultState(context));
        baseStateDictionary.Add(PlayerBaseState.Climb, new ClimbState(context));

        //climb states
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

        UpdateState(BaseState);
    }

    public void EnterState(PlayerBaseState baseState)
    {
        baseStateDictionary[baseState].Enter();
    }

    public void EnterState(PlayerClimbState climbState)
    {
        climbStateDictionary[climbState].Enter();
    }

    public void UpdateState(PlayerBaseState baseState)
    {
        baseStateDictionary[baseState].Update();
    }

    public void UpdateState(PlayerClimbState climbState)
    {
        climbStateDictionary[climbState].Update();
    }

    public void ExitState(PlayerBaseState baseState)
    {
        baseStateDictionary[baseState].Exit();
    }

    public void ExitState(PlayerClimbState climbState)
    {
        climbStateDictionary[climbState].Exit();
    }

    public void SetState(PlayerClimbState climbState)
    {
        SetState(PlayerBaseState.Climb, climbState);
    }

    public void SetState(PlayerBaseState newMoveState, PlayerClimbState newClimbState = PlayerClimbState.None)
    {

        ExitState(BaseState);

        Debug.Log($"Change state from: {BaseState} ({ClimbState}) to {newMoveState}({newClimbState}).");
        OnStateChangePrevious?.Invoke(BaseState, ClimbState, newMoveState, newClimbState);
        OnStateChange?.Invoke(newMoveState, newClimbState);

        BaseState = newMoveState;
        ClimbState = newClimbState;

        EnterState(newMoveState);

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

        GUILayout.Box(BaseState + " : " + ClimbState);
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
