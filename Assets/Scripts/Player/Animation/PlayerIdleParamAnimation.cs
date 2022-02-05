using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleParamAnimation : ParameterBasedAnimation<IdleState>
{
    [SerializeField] Sprite atWallSprite;
    [SerializeField] Sprite[] idleStand, idleCrouch;
    private Sprite[] idle => state.IsCrouching ? idleCrouch : idleStand;
    private IdleState state;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        state = PlayerStateMachine.Instance.CurrentState as IdleState;

        if (state.IsAtWall)
            return atWallSprite;

        return idle[(int)(Mathf.Floor(Time.time * (idle.Length + 0.5f)) % idle.Length)];
    }
}

