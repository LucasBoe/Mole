using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterBasedAnimationBase : ScriptableObject
{
    public FlipOverrides FlipOverride = FlipOverrides.None;
    public virtual void Init(PlayerStateMachine playerStateMachine) { }

    public virtual Sprite Update()
    {
        return null;
    }

    public enum FlipOverrides
    {
        None,
        DontUpdate,
        Left,
        Right,
    }

    public virtual void SetState(PlayerStateBase state) { }
}

public class ParameterBasedAnimation<T> : ParameterBasedAnimationBase where T : PlayerStateBase
{
    private T state;
    public T State
    {
        get => state;
    }

    public override void SetState(PlayerStateBase state)
    {
        this.state = state as T;
    }
}

[System.Serializable]
public class FloatSpritePair
{
    public float Value;
    public Sprite Sprite;
}
