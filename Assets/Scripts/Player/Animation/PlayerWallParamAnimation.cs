using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallParamAnimation : ParameterBasedAnimationBase
{
    private const PlayerState associatedState = PlayerState.Wall;
    private WallState wallState;

    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite[] climbing;
    [SerializeField] FloatSpritePair[] ledgeClimbing;

    float animSpeed;


    public override void Init(PlayerController playerController)
    {
        wallState = playerController.stateDictionary[associatedState] as WallState;
    }

    public override Sprite Update()
    {
        if (wallState.DistanceFromTop > 0)
        {
            foreach (FloatSpritePair pair in ledgeClimbing)
            {
                if (wallState.DistanceFromTop < pair.Value)
                    return pair.Sprite;
            }

            return ledgeClimbing[0].Sprite;
        }
        else
        {
            if (wallState.IsMoving)
                return climbing[(int)(Mathf.Floor(Time.time * (climbing.Length + 0.5f)) % climbing.Length)];

            return baseSprite;
        }
    }
}

