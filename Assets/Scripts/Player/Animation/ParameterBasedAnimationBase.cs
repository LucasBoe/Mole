using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParameterBasedAnimationBase : ScriptableObject
{
    public virtual void Init(PlayerStateMachine playerStateMachine)
    {

    }

    public virtual Sprite Update()
    {
        return null;
    }
}

public class ParameterBasedAnimation<T> : ParameterBasedAnimationBase where T : PlayerStateBase
{
    protected PlayerState StateType;
    protected T State;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        State = playerStateMachine.stateDictionary[StateType] as T;
    }
}

[System.Serializable]
public class FloatSpritePair
{
    public float Value;
    public Sprite Sprite;
}
