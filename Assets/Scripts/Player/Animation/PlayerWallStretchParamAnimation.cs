using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallStretchParamAnimation : ParameterBasedAnimation<WallStretchState>
{
    [SerializeField] FloatSpritePair[] wallStretch;
    WallStretchState state;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        state = PlayerStateMachine.Instance.CurrentState as WallStretchState;
        foreach (FloatSpritePair pair in wallStretch)
        {
            if (state.Distance < pair.Value)
                return pair.Sprite;
        }

        return wallStretch[0].Sprite;
    }
}

