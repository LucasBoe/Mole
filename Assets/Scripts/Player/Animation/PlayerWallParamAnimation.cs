using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWallParamAnimation : ParameterBasedAnimation<WallState>
{
    [SerializeField] Sprite baseSprite;
    [SerializeField] Sprite[] climbing;
    [SerializeField] FloatSpritePair[] ledgeClimbing;

    WallState state;

    float animSpeed;

    public override void Init(PlayerStateMachine playerStateMachine)
    {
        base.Init(playerStateMachine);
    }

    public override Sprite Update()
    {
        state = PlayerStateMachine.Instance.CurrentState as WallState;
        if (state.DistanceFromTop > 0)
        {
            foreach (FloatSpritePair pair in ledgeClimbing)
            {
                if (state.DistanceFromTop < pair.Value)
                    return pair.Sprite;
            }

            return ledgeClimbing[0].Sprite;
        }
        else
        {
            if (state.IsMoving)
                return climbing[(int)(Mathf.Floor(Time.time * (climbing.Length + 0.5f)) % climbing.Length)];

            return baseSprite;
        }
    }
}

