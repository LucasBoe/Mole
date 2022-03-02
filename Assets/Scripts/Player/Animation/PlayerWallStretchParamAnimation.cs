using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallStretchParamAnimation : ParameterBasedAnimation<WallStretchState>
{
    [SerializeField] FloatSpritePair[] wallStretch;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
        FlipOverride = FlipOverrides.DontUpdate;
    }

    public override Sprite Update()
    {
        foreach (FloatSpritePair pair in wallStretch)
        {
            if (State.Distance < pair.Value)
                return pair.Sprite;
        }

        return wallStretch[0].Sprite;
    }
}

