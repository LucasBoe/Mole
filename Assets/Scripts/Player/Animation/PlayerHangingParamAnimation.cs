using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParamAnimations/PlayerHangingParamAnimation")]
public class PlayerHangingParamAnimation : ParameterBasedAnimation<HangingState>
{
    [SerializeField] Sprite[] hanging;

    private Sprite sprite;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
        sprite = hanging[0];
    }

    public override Sprite Update()
    {
        if (State.IsMoving)
        {
            sprite = hanging[(int)(Mathf.Floor(Time.time * (hanging.Length + 0.5f)) % hanging.Length)];
            FlipOverride = State.XMoveDir < 0 ? FlipOverrides.Left : FlipOverrides.Right;
        }


        return sprite;
    }
}

