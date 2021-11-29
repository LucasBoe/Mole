using System;
using System.Collections;
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
    WallStretch,
}

public class PlayerStateMachine : SingletonBehaviour<PlayerStateMachine>, IPlayerComponent
{
    public PlayerState CurrentState;

    public System.Action<PlayerState> OnStateChange;

    public System.Action<PlayerState, PlayerState> OnStateChangePrevious;

    public Dictionary<PlayerState, PlayerStateBase> stateDictionary = new Dictionary<PlayerState, PlayerStateBase>();

    public int UpdatePrio => 50;

    public void Init(PlayerContext context)
    {
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
        stateDictionary.Add(PlayerState.WallStretch, new WallStretchState(context));

    }
    public void UpdatePlayerComponent(PlayerContext context)
    {
        UpdateState(CurrentState);
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