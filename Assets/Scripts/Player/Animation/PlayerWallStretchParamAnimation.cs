using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallStretchParamAnimation : ParameterBasedAnimationBase
{
    private const PlayerState associatedState = PlayerState.WallStretch;
    private WallStretchState wallState;

    [SerializeField] FloatSpritePair[] wallStretch;


    public override void Init(PlayerController playerController)
    {
        wallState = playerController.stateDictionary[associatedState] as WallStretchState;
    }

    public override Sprite Update()
    {
        foreach (FloatSpritePair pair in wallStretch)
        {
            if (wallState.Distance < pair.Value)
                return pair.Sprite;
        }

        return wallStretch[0].Sprite;
    }
}

