using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "ParamAnimations/PlayerWallStretchParamAnimation")]
public class PlayerWallStretchParamAnimation : ParameterBasedAnimation<GutterStretchState>
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

