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
    CombatStrangle,
    RopeClimb,
    Tunnel,
    InWindow,
    Hiding,
}

public class PlayerStateMachine : PlayerSingletonBehaviour<PlayerStateMachine>, IPlayerComponent
{
    public PlayerStateBase CurrentState;

    public System.Action<PlayerStateBase> OnStateChange;
    public System.Action<PlayerStateBase, PlayerStateBase> OnStateChangePrevious;

    public int UpdatePrio => 50;

    public void Init(PlayerContext context)
    {
        CurrentState = new IdleState();
    }
    public void UpdatePlayerComponent(PlayerContext context)
    {
        UpdateState(CurrentState);
    }

    public void SetStateDelayed(PlayerStateBase newState, float delay)
    {
        this.Delay(delay, () => SetState(newState));
    }

    public void SetState(PlayerStateBase newState)
    {
        if (!newState.CheckEnter())
        {
            LogWarning("could not enter: " + newState);
            return;
        }

        if (!CurrentState.CheckExit())
        {
            LogWarning("could not exit: " + CurrentState);
            return;
        }

        CurrentState.Exit();

        string from = CurrentState.ToString();
        string to = newState.ToString();

        Log($"Change state from: ({from}) to ({newState})");

        OnStateChangePrevious?.Invoke(CurrentState, newState);
        OnStateChange?.Invoke(newState);

        CurrentState = newState;
        newState.Enter();
    }

    public void UpdateState(PlayerStateBase state)
    {
        state.Update();
    }
}