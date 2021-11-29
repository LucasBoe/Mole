using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleParamAnimation : ParameterBasedAnimation<IdleState>
{
    [SerializeField] Sprite atWallSprite;
    [SerializeField] Sprite[] climbing;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        StateType = PlayerState.Idle;
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        if (State.IsAtWall)
            return atWallSprite;

        return climbing[(int)(Mathf.Floor(Time.time * (climbing.Length + 0.5f)) % climbing.Length)];
    }
}

