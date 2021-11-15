using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallParamAnimation : ParameterBasedAnimationBase
{
    private const PlayerClimbState associatedState = PlayerClimbState.Wall;
    private WallState wallState;

    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite[] climbing;
    [SerializeField] DistanceSpritePair[] ledgeClimbing;

    float animSpeed;


    public override void Init(PlayerController playerController)
    {
        wallState = playerController.climbStateDictionary[associatedState] as WallState;
    }

    public override Sprite Update()
    {
        if (wallState.DistanceFromTop > 0)
        {
            foreach (DistanceSpritePair pair in ledgeClimbing)
            {
                if (wallState.DistanceFromTop < pair.Distance)
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

[System.Serializable]
public class DistanceSpritePair
{
    public float Distance;
    public Sprite Sprite;
}

