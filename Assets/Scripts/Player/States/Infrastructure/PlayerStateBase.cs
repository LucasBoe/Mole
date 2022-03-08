using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerCollisionCheckType;
using System.Reflection;

public class PlayerStateObject
{
    protected PlayerContext context;
    public PlayerContext Context => context;
    public bool IsColliding(CheckType type)
    {
        return context.CollisionChecks[type].IsDetecting;
    }

    public bool IsColliding(params CheckType[] types)
    {
        foreach (var type in types)
        {
            if (context.CollisionChecks[type].IsDetecting)
                return true;
        }

        return false;
    }
    protected CollisionCheck GetCheck(CheckType type)
    {
        return context.CollisionChecks[type];
    }

    protected static void SetState(PlayerStateBase state)
    {
        PlayerStateMachine.Instance.SetState(state);
    }

    protected void SetStateDelayed(PlayerStateBase state, float delay = 1)
    {
        PlayerStateMachine.Instance.SetStateDelayed(state, delay);
    }

    protected bool StateIs(PlayerStateBase state)
    {
        return StateIs(state.GetType());
    }

    public bool StateIs(System.Type stateType)
    {
        return PlayerStateMachine.Instance.CurrentState.GetType() == stateType;
    }

    public PlayerStateObject()
    {
        context = PlayerController.Context;
    }
    protected void Watch(string name, string value)
    {
        ConsoleProDebug.Watch(name, value);
    }
}

//Player State Base Class with common and abstract functions 
public abstract class PlayerStateBase : PlayerStateObject
{
    public virtual bool CheckEnter()
    {
        bool canEnter = !(PlayerItemUser.Instance.IsHoldingHeavyItem() && !StateAllowsCarryingHeavyObjects);
        ConsoleProDebug.Watch("TryEnter: ", $"{this.GetType()} check enter: {canEnter} (holding heavy: {PlayerItemUser.Instance.IsHoldingHeavyItem()} && heavy allowed: {StateAllowsCarryingHeavyObjects}) ");
        return canEnter;
    }
    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }

    public virtual bool StateAllowsCarryingHeavyObjects => true;

    protected void SetCollisionActive(bool active)
    {
        PlayerPhysicsModifier.Instance.SetCollisionActive(active);
    }
    protected void SetGravityActive(bool active)
    {
        PlayerPhysicsModifier.Instance.SetGracityActive(active);
    }

    protected void SetPlayerConstrained(bool constrained)
    {
        PlayerPhysicsModifier.Instance.SetPlayerConstrained(constrained);
    }

    protected void SetPlayerDragActive(bool active)
    {
        PlayerPhysicsModifier.Instance.SetPlayerDragActive(active);
    }

    protected void SetHidingMode(PlayerHidingHandler.HidingMode mode)
    {
        PlayerHidingHandler.Instance.SetHidingMode(mode);
    }

    protected void SetHidingMode(HidingState hidingState)
    {
        PlayerHidingHandler.Instance.SetHidingMode(PlayerHidingHandler.HidingMode.StateDynamic, hidingState);
    }

    protected void JumpOff(Vector2 input)
    {
        context.Rigidbody.velocity = input * context.Values.JumpOffVelocity;
        SetState(new FallState());
    }
    protected void ApplyGravity(float time)
    {
        context.Rigidbody.AddForce(new Vector2(0, -Time.deltaTime * 1000f * context.Values.AdditionalGravityForce.Evaluate(Mathf.Clamp(time, 0f, 4f))));
    }
}
