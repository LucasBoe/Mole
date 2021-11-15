using PlayerCollisionCheckType;
using System.Collections.Generic;
using UnityEngine;

public enum PlayerState
{
    None,
    Wall,
    Ceiling,
    Hanging,
    PullUp,
    DropDown,
    JumpToHanging,
    Idle,
    Walk,
    Jump,
    Fall,
    WalkPush,
}

public class PlayerController : MonoBehaviour
{
    public PlayerState CurrentState;

    [SerializeField] PlayerContext context;

    bool jumpBlocker = false;

    [SerializeField] PlayerValues playerValues;

    public System.Action<PlayerState> OnStateChange;
    public System.Action<PlayerState, PlayerState> OnStateChangePrevious;

    public Dictionary<PlayerState, PlayerStateBase> stateDictionary = new Dictionary<PlayerState, PlayerStateBase>();

    private void Awake()
    {
        context = new PlayerContext();
        context.Rigidbody = GetComponent<Rigidbody2D>();
        context.PlayerController = this;
        context.Values = playerValues;

        //base
        context.CollisionChecks.Add(CheckType.Ground, new CollisionCheck(0f, -1.25f, 0.5f, 0.25f, LayerMask.GetMask("Default", "Hangable", "OneDirectionalFloor", "Pushable")));
        context.CollisionChecks.Add(CheckType.Hangable, new CollisionCheck(0f, 1.375f, 1.5f, 1f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableJumpInLeft, new CollisionCheck(-0.7f, 0.2f, 0.3f, 1.5f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableJumpInRight, new CollisionCheck(0.7f, 0.2f, 0.3f, 1.5f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.WallLeft, new CollisionCheck(-0.55f, 0, 0.45f, 1f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.WallRight, new CollisionCheck(0.55f, 0, 0.45f, 1f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.WallAbove, new CollisionCheck(0, 1.1f, 1.5f, 0.2f, LayerMask.GetMask("Default")));
        context.CollisionChecks.Add(CheckType.Ceiling, new CollisionCheck(0f, 0.875f, 0.75f, 0.25f, LayerMask.GetMask("Default", "OneDirectionalFloor")));
        context.CollisionChecks.Add(CheckType.Body, new CollisionCheck(0f, 0f, 0.875f, 1.5f, LayerMask.GetMask("Default", "Hangable")));

        //details
        context.CollisionChecks.Add(CheckType.HangableLeft, new CollisionCheck(-0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableRight, new CollisionCheck(0.75f, 1.5f, .5f, 1.25f, LayerMask.GetMask("Hangable")));
        context.CollisionChecks.Add(CheckType.HangableAboveAir, new CollisionCheck(0f, 2.875f, 1f, 2f, LayerMask.GetMask("Default", "Hangable")));
        context.CollisionChecks.Add(CheckType.DropDownable, new CollisionCheck(0, -1.5f, 0.5f, 1f, LayerMask.GetMask("Hangable","OneDirectionalFloor")));
        context.CollisionChecks.Add(CheckType.EdgeHelperLeft, new CollisionCheck(-0.5f, -0.75f, 0.5f, 0.75f, LayerMask.GetMask("Default", "Hangable", "Pushable")));
        context.CollisionChecks.Add(CheckType.EdgeHelperRight, new CollisionCheck(0.5f, -0.75f, 0.5f, 0.75f, LayerMask.GetMask("Default", "Hangable", "Pushable")));

        //pushable
        context.CollisionChecks.Add(CheckType.PushableLeft, new CollisionCheck(-0.75f, 0f, 0.5f, 0.75f, LayerMask.GetMask("Pushable")));
        context.CollisionChecks.Add(CheckType.PushableRight, new CollisionCheck(0.75f, 0f, 0.5f, 0.75f, LayerMask.GetMask("Pushable")));

        //move states
        stateDictionary.Add(PlayerState.Idle, new IdleState(context));
        stateDictionary.Add(PlayerState.Walk, new WalkState(context));
        stateDictionary.Add(PlayerState.WalkPush, new WalkPushState(context));
        stateDictionary.Add(PlayerState.Jump, new JumpState(context));
        stateDictionary.Add(PlayerState.Fall, new FallState(context));

        //climb states
        stateDictionary.Add(PlayerState.PullUp, new PullUpState(context));
        stateDictionary.Add(PlayerState.DropDown, new DropDownState(context));
        stateDictionary.Add(PlayerState.Hanging, new HangingState(context));
        stateDictionary.Add(PlayerState.JumpToHanging, new JumpToHangingState(context));
        stateDictionary.Add(PlayerState.Wall, new WallState(context));
    }

    // Update is called once per frame
    void Update()
    {
        context.PlayerPos = transform.position;
        context.IsCollidingToAnyWall = IsColliding(CheckType.WallLeft) || IsColliding(CheckType.WallRight);
        context.Input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        context.TriesMoveLeftRight = context.Input.x != 0;
        context.TriesMoveUpDown = context.Input.y != 0f;
        context.IsJumping = Input.GetButtonDown("Jump");

        UpdateState(CurrentState);
    }

    private void FixedUpdate()
    {
        foreach (CollisionCheck pcc in context.CollisionChecks.Values)
            pcc.Update(transform);
    }

    public void EnterState(PlayerState newState)
    {
        if (newState != PlayerState.None)
            stateDictionary[newState].Enter();
    }

    public void UpdateState(PlayerState newState)
    {
        if (newState != PlayerState.None)
            stateDictionary[newState].Update();
    }

    //Exit Methods
    public void ExitState(PlayerState newState)
    {
        if (newState != PlayerState.None)
            stateDictionary[newState].Exit();
    }

    public void SetState(PlayerState newState)
    {

        ExitState(CurrentState);

        string from = CurrentState.ToString();
        string to = newState.ToString();
        Debug.Log($"Change state from: ({from}) to ({newState})");
        OnStateChangePrevious?.Invoke(CurrentState, newState);
        OnStateChange?.Invoke(newState);

        CurrentState = newState;

        EnterState(newState);

    }

    private bool IsColliding(CheckType checkType)
    {
        return context.CollisionChecks[checkType].IsDetecting;
    }

    private void OnDrawGizmos()
    {
        if (context == null || context.CollisionChecks == null)
            return;

        foreach (CollisionCheck pcc in context.CollisionChecks.Values)
        {
            Util.GizmoDrawCollisionCheck(pcc, transform.position);
        }

        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere((Vector2)transform.position + playerValues.HangableOffset, 0.25f);
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

        GUILayout.Box(CurrentState.ToString());
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
        HangableJumpInLeft,
        HangableJumpInRight,
        HangableAboveAir,
        Body,
        DropDownable,
        EdgeHelperLeft,
        EdgeHelperRight,
        PushableLeft,
        PushableRight,
        WallAbove,
    }
}
