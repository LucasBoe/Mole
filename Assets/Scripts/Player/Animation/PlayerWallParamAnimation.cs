using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallParamAnimation : ParameterBasedAnimation<WallState>
{
    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite[] climbing;
    [SerializeField] FloatSpritePair[] ledgeClimbing;

    float animSpeed;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        StateType = PlayerState.Wall;
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        if (State.DistanceFromTop > 0)
        {
            foreach (FloatSpritePair pair in ledgeClimbing)
            {
                if (State.DistanceFromTop < pair.Value)
                    return pair.Sprite;
            }

            return ledgeClimbing[0].Sprite;
        }
        else
        {
            if (State.IsMoving)
                return climbing[(int)(Mathf.Floor(Time.time * (climbing.Length + 0.5f)) % climbing.Length)];

            return baseSprite;
        }
    }
}

